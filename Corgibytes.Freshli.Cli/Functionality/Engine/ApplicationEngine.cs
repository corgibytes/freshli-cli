using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class ApplicationEngine : IApplicationEventEngine, IApplicationActivityEngine
{
    private const int MutexWaitTimeoutInMilliseconds = 50;
    private static readonly Dictionary<Type, Action<IApplicationEvent>> s_eventHandlers = new();

    private static readonly object s_dispatchLock = new();
    private static readonly object s_fireLock = new();
    private static bool s_isActivityDispatchingInProgress;
    private static bool s_isEventFiringInProgress;
    private readonly ILogger<ApplicationEngine> _logger;

    public ApplicationEngine(IBackgroundJobClient jobClient, ILogger<ApplicationEngine> logger,
        IServiceProvider serviceProvider)
    {
        JobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));
        _logger = logger;
        ServiceProvider = serviceProvider;
    }

    private IBackgroundJobClient JobClient { get; }

    public void Dispatch(IApplicationActivity applicationActivity)
    {
        lock (s_dispatchLock)
        {
            s_isActivityDispatchingInProgress = true;
            try
            {
                JobClient.Enqueue(() => HandleActivity(applicationActivity));
            }
            finally
            {
                s_isActivityDispatchingInProgress = false;
            }
        }
    }

    public void Wait()
    {
        var watch = new Stopwatch();
        watch.Start();
        LogWaitStart();

        var shouldWait = true;
        while (shouldWait)
        {
            Thread.Sleep(500);

            var statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
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

    public void Fire(IApplicationEvent applicationEvent)
    {
        lock (s_fireLock)
        {
            s_isEventFiringInProgress = true;
            try
            {
                JobClient.Enqueue(() => FireEventAndHandler(applicationEvent));
            }
            finally
            {
                s_isEventFiringInProgress = false;
            }
        }
    }


    public void On<TEvent>(Action<TEvent> eventHandler) where TEvent : IApplicationEvent =>
        s_eventHandlers.Add(typeof(TEvent), boxedEvent => eventHandler((TEvent)boxedEvent));

    private void LogWaitStart() => _logger.LogDebug("Starting to wait for an empty job queue...");

    private void LogWaitStop(long durationInMilliseconds) =>
        _logger.LogDebug("Waited for {Duration} milliseconds", durationInMilliseconds);

    private void LogWaitingStatus(StatisticsDto statistics, long length, bool localIsEventFiring,
        bool localIsActivityDispatching) =>
        _logger.LogTrace(
            "Queue length: {QueueLength} (" +
            "Processing: {JobsProcessing}, " +
            "Enqueued: {JobsEnqueued}, " +
            "Succeeded: {JobsSucceeded}, " +
            "Scheduled: {JobsScheduled}, " +
            "Failed: {JobsFailed}), " +
            "Activity Dispatch in Progress: {IsActivityDispatchInProgress}, " +
            "Event Fire in Progress: {IsEventFireInProgress}",
            length,
            statistics.Processing,
            statistics.Enqueued,
            statistics.Succeeded,
            statistics.Scheduled,
            statistics.Failed,
            localIsActivityDispatching,
            localIsEventFiring
        );

    // ReSharper disable once MemberCanBePrivate.Global
    public void FireEventAndHandler(IApplicationEvent applicationEvent)
    {
        var jobId = JobClient.Enqueue(() => HandleEvent(applicationEvent));

        foreach (var _ in s_eventHandlers.Keys.Where(type => type.IsAssignableTo(applicationEvent.GetType())))
        {
            JobClient.ContinueJobWith(jobId, () => TriggerHandler(applicationEvent));
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void HandleActivity(IApplicationActivity activity)
    {
        Mutex? mutex = null;
        if (activity is IMutexed mutexSource)
        {
            mutex = mutexSource.GetMutex(ServiceProvider);
        }

        var mutexAcquired = mutex?.WaitOne(MutexWaitTimeoutInMilliseconds) ?? true;
        if (!mutexAcquired)
        {
            _logger.LogDebug(
                "[{ThreadId}] Re-Dispatch Activity - {ActivityType}: {Activity}",
                Thread.CurrentThread.ManagedThreadId,
                activity.GetType(),
                SerializationHelper.Serialize(activity)
            );

            // place the activity back in the queue and free up the worker to make progress on a different activity
            Dispatch(activity);
            return;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        _logger.LogDebug(
            "[{ThreadId}] Start Activity - {ActivityType}: {Activity}",
            Thread.CurrentThread.ManagedThreadId,
            activity.GetType(),
            SerializationHelper.Serialize(activity)
        );

        try
        {
            activity.Handle(this);
        }
        catch (Exception error)
        {
            Fire(new UnhandledExceptionEvent(error));
        }
        finally
        {
            if (mutex != null && mutexAcquired)
            {
                mutex.ReleaseMutex();
            }
        }

        stopwatch.Stop();
        _logger.LogDebug(
            "[{ThreadId}] (Took {Time}ms) End Activity - {ActivityType}: {Activity}",
            Thread.CurrentThread.ManagedThreadId,
            stopwatch.ElapsedMilliseconds,
            activity.GetType(),
            SerializationHelper.Serialize(activity)
        );
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void HandleEvent(IApplicationEvent appEvent)
    {
        var stopwatch = new Stopwatch();
        _logger.LogDebug(
            "[{Thread}] Start Event - {AppEventType}: {AppEvent}",
            Thread.CurrentThread.ManagedThreadId,
            appEvent.GetType(),
            SerializationHelper.Serialize(appEvent)
        );

        try
        {
            appEvent.Handle(this);
        }
        catch (Exception error)
        {
            Fire(new UnhandledExceptionEvent(error));
        }

        stopwatch.Stop();
        _logger.LogDebug(
            "[{ThreadId}] (Took {Time}ms) End Event - {EventType}: {Event}",
            Thread.CurrentThread.ManagedThreadId,
            stopwatch.ElapsedMilliseconds,
            appEvent.GetType(),
            SerializationHelper.Serialize(appEvent)
        );
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void TriggerHandler(IApplicationEvent applicationEvent)
    {
        foreach (var type in s_eventHandlers.Keys)
        {
            if (!type.IsAssignableTo(applicationEvent.GetType()))
            {
                continue;
            }

            var handler = s_eventHandlers[type];
            handler(applicationEvent);
        }
    }
}
