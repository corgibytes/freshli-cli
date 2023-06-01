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

    private readonly ICountdownEvent _queueModificationsCountdownEvent = new DefaultCountdownEvent(0);
    private readonly ILogger<ApplicationEngine> _logger;

    private readonly ConcurrentDictionary<IApplicationTask, ICountdownEvent> _tasksAndCountdownEvents = new();

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
        lock (_queueModificationsCountdownEvent)
        {
            _tasksAndCountdownEvents.TryAdd(
                task,
                new ListeningCountdownEvent(_queueModificationsCountdownEvent, 1)
            );
        }
    }

    public async ValueTask Dispatch(IApplicationActivity applicationActivity, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        lock (_queueModificationsCountdownEvent)
        {
            if (!_queueModificationsCountdownEvent.TryAddCount())
            {
                _queueModificationsCountdownEvent.Reset(1);
            }
        }

        RecordTask(applicationActivity);

        try
        {
            await TaskQueue.QueueBackgroundWorkItemAsync(new WorkItem(applicationActivity,
                _ => HandleActivity(applicationActivity, mode, cancellationToken)), cancellationToken);
        }
        finally
        {
            lock (_queueModificationsCountdownEvent)
            {
                _queueModificationsCountdownEvent.Signal();
            }
        }
    }

    public ValueTask Wait(IApplicationTask task, CancellationToken cancellationToken)
    {
        var timer = new Stopwatch();
        timer.Start();

        LogWaitStart(task);
        ICountdownEvent? countdownEvent;

        while (!_tasksAndCountdownEvents.TryGetValue(task, out countdownEvent))
        {
            Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
        }

        countdownEvent.Wait(cancellationToken);

        timer.Stop();
        LogWaitStop(timer.ElapsedMilliseconds);

        return ValueTask.CompletedTask;
    }

    public IServiceProvider ServiceProvider { get; }

    public async ValueTask Fire(IApplicationEvent applicationEvent, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        lock (_queueModificationsCountdownEvent)
        {
            if (!_queueModificationsCountdownEvent.TryAddCount())
            {
                _queueModificationsCountdownEvent.Reset(1);
            }
        }

        RecordTask(applicationEvent);

        try
        {
            await TaskQueue.QueueBackgroundWorkItemAsync(new WorkItem(applicationEvent,
                _ => FireEventAndHandler(applicationEvent, mode, cancellationToken)), cancellationToken);
        }
        finally
        {
            lock (_queueModificationsCountdownEvent)
            {
                _queueModificationsCountdownEvent.Signal();
            }
        }
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
        ICountdownEvent? countdownEvent;
        while (!_tasksAndCountdownEvents.TryGetValue(applicationEvent, out countdownEvent))
        {
            await Task.Delay(10, cancellationToken);
        }

        try
        {
            await HandleEvent(applicationEvent, mode, cancellationToken);

            bool applicationEventHasHandlers;
            lock (_eventHandlers)
            {
                applicationEventHasHandlers =
                    _eventHandlers.Keys.Any(type => type.IsAssignableTo(applicationEvent.GetType()));
            }
            if (applicationEventHasHandlers)
            {
                countdownEvent.AddCount();

                // TODO: Pass the cancellation token to TriggerHandler
                await TaskQueue.QueueBackgroundWorkItemAsync(new WorkItem(applicationEvent,
                    _ => TriggerHandler(applicationEvent)), cancellationToken);
            }
        }
        finally
        {
            countdownEvent.Signal();
        }
    }

    private async ValueTask HandleActivity(IApplicationActivity activity, ApplicationTaskMode mode, CancellationToken cancellationToken)
    {
        SemaphoreSlim? semaphore = null;
        if (activity is ISynchronized mutexSource)
        {
            semaphore = await mutexSource.GetSemaphore(ServiceProvider);
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

        ChildTrackingApplicationEngine eventEngine;
        lock (_queueModificationsCountdownEvent)
        {
            eventEngine = new ChildTrackingApplicationEngine(
                this,
                _tasksAndCountdownEvents,
                _queueModificationsCountdownEvent,
                activity
            );
        }

        try
        {
            _logger.LogDebug(
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
            ICountdownEvent? countdownEvent;
            while (!_tasksAndCountdownEvents.TryGetValue(activity, out countdownEvent))
            {
                await Task.Delay(10, cancellationToken);
            }

            countdownEvent.Signal();

            if (semaphore != null && semaphoreEntered)
            {
                semaphore.Release();
            }
        }
    }

    private async ValueTask HandleEvent(IApplicationEvent appEvent, ApplicationTaskMode mode, CancellationToken cancellationToken)
    {
        ChildTrackingApplicationEngine eventEngine;
        lock (_queueModificationsCountdownEvent)
        {
            eventEngine = new ChildTrackingApplicationEngine(
                this,
                _tasksAndCountdownEvents,
                _queueModificationsCountdownEvent,
                appEvent
            );
        }

        try
        {
            // TODO: figure out how best to log event specific values here
            _logger.LogDebug(
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

    private async ValueTask TriggerHandler(IApplicationEvent applicationEvent)
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

        ICountdownEvent? countdownEvent;
        while (!_tasksAndCountdownEvents.TryGetValue(applicationEvent, out countdownEvent))
        {
            await Task.Delay(10);
        }

        countdownEvent.Signal();
    }

    public void Dispose()
    {
        _queueModificationsCountdownEvent.Dispose();

        GC.SuppressFinalize(this);
    }
}
