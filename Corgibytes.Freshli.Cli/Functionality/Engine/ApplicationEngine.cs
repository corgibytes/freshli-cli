using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core.Tokens;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class ApplicationEngine : IApplicationEventEngine, IApplicationActivityEngine
{
    private const int MutexWaitTimeoutInMilliseconds = 50;
    private static readonly Dictionary<Type, Func<IApplicationEvent, ValueTask>> s_eventHandlers = new();

    private static readonly SemaphoreSlim s_dispatchSemaphore = new(1, 1);
    private static readonly SemaphoreSlim s_fireSemaphore = new(1, 1);
    private static bool s_isActivityDispatchingInProgress;
    private static bool s_isEventFiringInProgress;
    private readonly ILogger<ApplicationEngine> _logger;

    public ApplicationEngine(IBackgroundTaskQueue jobClient, ILogger<ApplicationEngine> logger,
        IServiceProvider serviceProvider)
    {
        JobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));
        _logger = logger;
        ServiceProvider = serviceProvider;
    }

    private IBackgroundTaskQueue JobClient { get; }

    public async ValueTask Dispatch(IApplicationActivity applicationActivity)
    {
        await s_dispatchSemaphore.WaitAsync();
        s_isActivityDispatchingInProgress = true;

        try
        {
            // TODO: Pass the cancellation token to HandleActivity
            await JobClient.QueueBackgroundWorkItemAsync(_ => HandleActivity(applicationActivity));
        }
        finally
        {
            s_isActivityDispatchingInProgress = false;
            s_dispatchSemaphore.Release();
        }
    }

    public async ValueTask Wait()
    {
        var watch = new Stopwatch();
        watch.Start();
        LogWaitStart();

        var shouldWait = true;
        while (shouldWait)
        {
            await Task.Delay(500);

            var statistics = JobClient.GetStatistics();
            var length = statistics.Processing + statistics.Enqueued;

            // store the value of static boolean fields to avoid a race condition between the output of those values
            // and the assignment of the `shouldWait` variable
            var localIsEventFiring = s_isEventFiringInProgress;
            var localIsActivityDispatching = s_isActivityDispatchingInProgress;

            LogWaitingStatus(statistics, length, localIsEventFiring, localIsActivityDispatching);
            shouldWait = length > 0 || localIsActivityDispatching || localIsEventFiring;
        }

        watch.Stop();
        LogWaitStop(watch.ElapsedMilliseconds);
    }

    public IServiceProvider ServiceProvider { get; }

    public async ValueTask Fire(IApplicationEvent applicationEvent)
    {
        await s_fireSemaphore.WaitAsync();
        s_isEventFiringInProgress = true;
        try
        {
            // TODO: pass cancellation token to FireEventAndHandler
            await JobClient.QueueBackgroundWorkItemAsync(_ => FireEventAndHandler(applicationEvent));
        }
        finally
        {
            s_isEventFiringInProgress = false;
            s_fireSemaphore.Release();
        }
    }

    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent =>
        s_eventHandlers.Add(typeof(TEvent), boxedEvent => eventHandler((TEvent)boxedEvent));

    private void LogWaitStart() => _logger.LogDebug("Starting to wait for an empty job queue...");

    private void LogWaitStop(long durationInMilliseconds) =>
        _logger.LogDebug("Waited for {Duration} milliseconds", durationInMilliseconds);

    private void LogWaitingStatus(QueueStatistics statistics, long length, bool localIsEventFiring,
        bool localIsActivityDispatching) =>
        _logger.LogTrace(
            "Queue length: {QueueLength} (" +
            "Processing: {JobsProcessing}, " +
            "Enqueued: {JobsEnqueued}, " +
            "Succeeded: {JobsSucceeded}, " +
            "Failed: {JobsFailed}), " +
            "Activity Dispatch in Progress: {IsActivityDispatchInProgress}, " +
            "Event Fire in Progress: {IsEventFireInProgress}",
            length,
            statistics.Processing,
            statistics.Enqueued,
            statistics.Succeeded,
            statistics.Failed,
            localIsActivityDispatching,
            localIsEventFiring
        );

    // ReSharper disable once MemberCanBePrivate.Global
    public async ValueTask FireEventAndHandler(IApplicationEvent applicationEvent)
    {
        await HandleEvent(applicationEvent);

        if (s_eventHandlers.Keys.Any(type => type.IsAssignableTo(applicationEvent.GetType())))
        {
            // TODO: Pass the cancellation token to TriggerHandler
            await JobClient.QueueBackgroundWorkItemAsync(_ => TriggerHandler(applicationEvent));
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public async ValueTask HandleActivity(IApplicationActivity activity)
    {
        Mutex? mutex = null;
        if (activity is IMutexed mutexSource)
        {
            mutex = mutexSource.GetMutex(ServiceProvider);
        }

        var mutexAcquired = mutex?.WaitOne(MutexWaitTimeoutInMilliseconds) ?? true;
        if (!mutexAcquired)
        {
            // place the activity back in the queue and free up the worker to make progress on a different activity
            await Dispatch(activity);
            return;
        }

        try
        {
            _logger.LogDebug(
                "Handling activity {ActivityType}: {Activity}",
                activity.GetType(),
                // todo: figure out how best to log activity specific values here
                activity
            );
            await activity.Handle(this);
        }
        catch (Exception error)
        {
            await Fire(new UnhandledExceptionEvent(error));
        }
        finally
        {
            if (mutex != null && mutexAcquired)
            {
                mutex.ReleaseMutex();
            }
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public async ValueTask HandleEvent(IApplicationEvent appEvent)
    {
        try
        {
            _logger.LogDebug(
                "Handling activity {AppEventType}: {AppEvent}",
                appEvent.GetType(),
                // todo: figure out how best to log event specific values here
                appEvent
            );
            await appEvent.Handle(this);
        }
        catch (Exception error)
        {
            await Fire(new UnhandledExceptionEvent(error));
        }
    }

    // TODO: see if these methods can be made private now
    // ReSharper disable once MemberCanBePrivate.Global
    public async ValueTask TriggerHandler(IApplicationEvent applicationEvent)
    {
        var tasks = new List<Task>();
        foreach (var type in s_eventHandlers.Keys)
        {
            if (!type.IsAssignableTo(applicationEvent.GetType()))
            {
                continue;
            }

            var handler = s_eventHandlers[type];
            var task = handler(applicationEvent);
            tasks.Add(task.AsTask());
        }

        Task.WaitAll(tasks.ToArray());
    }
}
