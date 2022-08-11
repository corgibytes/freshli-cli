using System;
using System.Collections.Generic;
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

    public void Dispatch(IApplicationActivity applicationActivity)
    {
        var statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
        var length = statistics.Servers;
        while (length > 0)
        {
            // TODO: Turn this into a Debug log call
            // Console.Out.WriteLine("Queue length: " + length + " (Processing: " + statistics.Processing + ", Enqueued: " + statistics.Enqueued + ")");
            statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
            length = statistics.Servers;
            Thread.Sleep(10);
        }

        JobClient.Enqueue(() => HandleActivity(applicationActivity));
    }

    public void Wait()
    {
        Thread.Sleep(100);

        var statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
        var length = statistics.Processing + statistics.Enqueued;
        while (length > 0)
        {
            // TODO: Turn this into a Debug log call
            // Console.Out.WriteLine("Queue length: " + length + " (Processing: " + statistics.Processing + ", Enqueued: " + statistics.Enqueued + ")");
            statistics = JobStorage.Current.GetMonitoringApi().GetStatistics();
            length = statistics.Processing + statistics.Enqueued;
            Thread.Sleep(10);
        }
    }

    public void Fire(IApplicationEvent applicationEvent)
    {
        var jobId = JobClient.Enqueue(() => HandleEvent(applicationEvent));

        foreach (var _ in s_eventHandlers.Keys.Where(type => type.IsAssignableTo(applicationEvent.GetType())))
        {
            JobClient.ContinueJobWith(jobId, () => TriggerHandler(applicationEvent));
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
