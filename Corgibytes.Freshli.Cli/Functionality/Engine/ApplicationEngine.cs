using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Hangfire;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class ApplicationEngine : IApplicationEventEngine, IApplicationActivityEngine
{
    private static readonly Dictionary<Type, Action<IApplicationEvent>> s_eventHandlers = new();

    private static readonly object s_dispatchLock = new();
    private static readonly object s_fireLock = new();
    private static bool s_isActivityDispatchingInProgress;
    private static bool s_isEventFiringInProgress;

    public ApplicationEngine(IBackgroundJobClient jobClient) =>
        JobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));

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
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Starting to wait for an empty job queue...");

        var shouldWait = true;
        while (shouldWait)
        {
            Thread.Sleep(10);

            var statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
            var length = statistics.Processing + statistics.Enqueued;

            // store the value of static boolean fields to avoid a race condition between the output of those values
            // and the assignment of the `shouldWait` variable
            var localIsEventFiring = s_isEventFiringInProgress;
            var localIsActivityDispatching = s_isActivityDispatchingInProgress;

            // TODO: Turn this into a Debug log call
            Console.Out.WriteLine("Queue length: " + length + " (Processing: " + statistics.Processing +
                                  ", Enqueued: " + statistics.Enqueued + ", Succeeded: " + statistics.Succeeded +
                                  ", Failed: " + statistics.Failed + "), Activity Dispatch in Progress: " +
                                  localIsActivityDispatching + ", Event Fire in Progress: " + localIsEventFiring);
            shouldWait = length > 0 || localIsActivityDispatching || localIsEventFiring;
        }

        watch.Stop();
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Waited for {0} millseconds", watch.ElapsedMilliseconds);
    }

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
    public void HandleActivity(IApplicationActivity activity) => activity.Handle(this);

    // ReSharper disable once MemberCanBePrivate.Global
    public void HandleEvent(IApplicationEvent appEvent) => appEvent.Handle(this);

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
