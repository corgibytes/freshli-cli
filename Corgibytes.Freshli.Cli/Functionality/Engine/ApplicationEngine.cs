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

    public ApplicationEngine(IBackgroundJobClient jobClient) =>
        JobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));

    private IBackgroundJobClient JobClient { get; }

    private bool _isActivityDispatchingInProgress;
    private bool _isEventFiringInProgress;

    public void Dispatch(IApplicationActivity applicationActivity)
    {
        _isActivityDispatchingInProgress = true;
        try
        {
            JobClient.Enqueue(() => HandleActivity(applicationActivity));
        }
        finally
        {
            _isActivityDispatchingInProgress = false;
        }
    }

    public void Wait()
    {
        var watch = new Stopwatch();
        watch.Start();
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Starting to wait for an empty job queue...");

        var statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
        var length = statistics.Processing + statistics.Enqueued;
        // TODO: Turn this into a Debug log call
        Console.Out.WriteLine("Queue length: " + length + " (Processing: " + statistics.Processing + ", Enqueued: " + statistics.Enqueued + ", Succeeded: " + statistics.Succeeded + ", Failed: " + statistics.Failed + "), Activity Dispatch in Progress: " + _isActivityDispatchingInProgress + ", Event Fire in Progress: " + _isEventFiringInProgress);
        while (length > 0 && !_isActivityDispatchingInProgress && !_isEventFiringInProgress)
        {
            Thread.Sleep(10);

            statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
            length = statistics.Processing + statistics.Enqueued;
            // TODO: Turn this into a Debug log call
            Console.Out.WriteLine("Queue length: " + length + " (Processing: " + statistics.Processing + ", Enqueued: " + statistics.Enqueued + ", Succeeded: " + statistics.Succeeded + ", Failed: " + statistics.Failed + "), Activity Dispatch in Progress: " + _isActivityDispatchingInProgress + ", Event Fire in Progress: " + _isEventFiringInProgress);
        }

        watch.Stop();
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Waited for {0} millseconds", watch.ElapsedMilliseconds);
    }

    public void Fire(IApplicationEvent applicationEvent)
    {
        _isEventFiringInProgress = true;
        try
        {
            var jobId = JobClient.Enqueue(() => HandleEvent(applicationEvent));

            foreach (var _ in s_eventHandlers.Keys.Where(type => type.IsAssignableTo(applicationEvent.GetType())))
            {
                JobClient.ContinueJobWith(jobId, () => TriggerHandler(applicationEvent));
            }
        }
        finally
        {
            _isEventFiringInProgress = false;
        }
    }



    public void On<TEvent>(Action<TEvent> eventHandler) where TEvent : IApplicationEvent =>
        s_eventHandlers.Add(typeof(TEvent), boxedEvent => eventHandler((TEvent)boxedEvent));

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
