using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class ApplicationEngine : IApplicationEventEngine, IApplicationActivityEngine, IDisposable
{
    // TODO: Make this a configurable value
    private const int MutexWaitTimeoutInMilliseconds = 200;
    private readonly Dictionary<Type, Func<IApplicationEvent, ValueTask>> _eventHandlers = new();

    private readonly ILogger<ApplicationEngine> _logger;

    private readonly ConcurrentDictionary<IApplicationTask, ApplicationTaskWaitToken> _tasksAndResetEvents = new();

    public ApplicationEngine(IBackgroundTaskQueue taskQueue, ILogger<ApplicationEngine> logger,
        IServiceProvider serviceProvider)
    {
        TaskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
        _logger = logger;
        ServiceProvider = serviceProvider;
    }

    private IBackgroundTaskQueue TaskQueue { get; }

    private void RecordTask(IApplicationTask task)
    {
        // Note: This will only return false if the task has already been recorded as a child task
        _tasksAndResetEvents.TryAdd(task, new ApplicationTaskWaitToken());
    }

    public async ValueTask Dispatch(IApplicationActivity applicationActivity, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        RecordTask(applicationActivity);

        await TaskQueue.QueueBackgroundWorkItemAsync(new WorkItem(applicationActivity,
            _ => HandleActivity(applicationActivity, mode, cancellationToken)), cancellationToken);
    }

    public async ValueTask Wait(IApplicationTask task, CancellationToken cancellationToken, ApplicationTaskWaitToken? excluding = null)
    {
        var timer = new Stopwatch();
        timer.Start();

        LogWaitStart(task);
        await WaitTaskCountdownEvent(task, cancellationToken, excluding);

        timer.Stop();
        LogWaitStop(timer.ElapsedMilliseconds);
    }

    public IServiceProvider ServiceProvider { get; }

    public async ValueTask Fire(IApplicationEvent applicationEvent, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        RecordTask(applicationEvent);

        await TaskQueue.QueueBackgroundWorkItemAsync(new WorkItem(applicationEvent,
            _ => FireEventAndHandler(applicationEvent, mode, cancellationToken)), cancellationToken);
    }

    public async ValueTask RegisterChildWaitToken(IApplicationTask task, ApplicationTaskWaitToken waitToken, CancellationToken cancellationToken)
    {
        var taskWaitToken = await GetTaskWaitInfo(task, cancellationToken);
        taskWaitToken.AddChildResetEvent(waitToken);
    }

    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent
    {
        lock (_eventHandlers)
        {
            _eventHandlers.Add(typeof(TEvent), boxedEvent => eventHandler((TEvent)boxedEvent));
        }
    }

    private void LogWaitStart(IApplicationTask task) =>
        _logger.LogDebug("Starting to wait for {Task} to complete...", task);

    private void LogWaitStop(long durationInMilliseconds) =>
        _logger.LogDebug("Waited for {Duration} milliseconds", durationInMilliseconds);

    private async ValueTask FireEventAndHandler(IApplicationEvent applicationEvent, ApplicationTaskMode mode, CancellationToken cancellationToken)
    {
        var applicationEventHasHandlers = false;
        try
        {
            await HandleEvent(applicationEvent, mode, cancellationToken);

            lock (_eventHandlers)
            {
                applicationEventHasHandlers =
                    _eventHandlers.Keys.Any(type => type.IsAssignableTo(applicationEvent.GetType()));
            }
            if (applicationEventHasHandlers)
            {
                // TODO: Pass the cancellation token to TriggerHandler
                await TaskQueue.QueueBackgroundWorkItemAsync(new WorkItem(applicationEvent,
                    _ => TriggerHandler(applicationEvent, cancellationToken)), cancellationToken);
            }
        }
        finally
        {
            if (!applicationEventHasHandlers)
            {
                await SignalTaskCountdownEvent(applicationEvent, cancellationToken);
            }
        }
    }

    private async ValueTask HandleActivity(IApplicationActivity activity, ApplicationTaskMode mode, CancellationToken cancellationToken)
    {
        SemaphoreSlim? semaphore = null;
        if (activity is ISynchronized mutexSource)
        {
            semaphore = mutexSource.GetSemaphore();
        }

        var semaphoreEntered = true;
        if (semaphore != null)
        {
            semaphoreEntered = await semaphore.WaitAsync(MutexWaitTimeoutInMilliseconds, cancellationToken);
        }
        if (!semaphoreEntered)
        {
            // place the activity back in the queue and free up the worker to make progress on a different activity
            await Dispatch(activity, cancellationToken, mode);
            return;
        }

        var eventEngine = new ChildTrackingApplicationEngine(
            this,
            _tasksAndResetEvents,
            activity
        );

        try
        {
            _logger.LogTrace(
                "Handling activity {ActivityType}: {Activity}",
                activity.GetType(),
                // TODO: figure out how best to log activity specific values here
                activity
            );
            await activity.Handle(eventEngine, cancellationToken);
        }
        catch (Exception error)
        {
            await eventEngine.Fire(new UnhandledExceptionEvent(error), cancellationToken, mode);
        }
        finally
        {
            await SignalTaskCountdownEvent(activity, cancellationToken);

            if (semaphore != null && semaphoreEntered)
            {
                semaphore.Release();
            }
        }
    }

    private async ValueTask HandleEvent(IApplicationEvent appEvent, ApplicationTaskMode mode, CancellationToken cancellationToken)
    {
        var eventEngine = new ChildTrackingApplicationEngine(
            this,
            _tasksAndResetEvents,
            appEvent
        );

        try
        {
            // TODO: figure out how best to log event specific values here
            _logger.LogTrace(
                "Handling activity {AppEventType}: {AppEvent}",
                appEvent.GetType(),
                appEvent
            );
            await appEvent.Handle(eventEngine, cancellationToken);
        }
        catch (Exception error)
        {
            await eventEngine.Fire(new UnhandledExceptionEvent(error), cancellationToken, mode);
        }
    }

    private async ValueTask TriggerHandler(IApplicationEvent applicationEvent, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        ICollection<Type> keys;
        lock (_eventHandlers)
        {
            keys = _eventHandlers.Keys;
        }
        foreach (var type in keys)
        {
            if (!type.IsAssignableTo(applicationEvent.GetType()))
            {
                continue;
            }

            Func<IApplicationEvent, ValueTask> handler;
            lock (_eventHandlers)
            {
                handler = _eventHandlers[type];
            }
            var task = handler(applicationEvent);
            tasks.Add(task.AsTask());
        }

        await Task.WhenAll(tasks);
        await SignalTaskCountdownEvent(applicationEvent, cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private async Task<ApplicationTaskWaitToken> GetTaskWaitInfo(IApplicationTask task, CancellationToken cancellationToken)
    {
        ApplicationTaskWaitToken? waitInfo;
        while (!_tasksAndResetEvents.TryGetValue(task, out waitInfo))
        {
            await Task.Delay(10, cancellationToken);
        }

        return waitInfo;
    }

    private async Task SignalTaskCountdownEvent(IApplicationTask task, CancellationToken cancellationToken)
    {
        var waitInfo = await GetTaskWaitInfo(task, cancellationToken);
        waitInfo.Signal();
    }

    private async Task WaitTaskCountdownEvent(IApplicationTask task, CancellationToken cancellationToken, ApplicationTaskWaitToken? excluding = null)
    {
        var waitInfo = await GetTaskWaitInfo(task, cancellationToken);
        await waitInfo.Wait(cancellationToken, excluding);
    }
}
