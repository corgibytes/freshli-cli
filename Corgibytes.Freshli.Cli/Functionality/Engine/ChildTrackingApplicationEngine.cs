using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class ChildTrackingApplicationEngine : IApplicationActivityEngine, IApplicationEventEngine
{
    private readonly IApplicationEngine _engine;
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly IApplicationEventEngine _eventEngine;
    private readonly IApplicationTask _parentTask;
    private readonly ConcurrentDictionary<IApplicationTask, ICountdownEvent> _tasksAndCountdownEvents;
    private readonly ICountdownEvent _queueModificationsCountdownEvent;

    public ChildTrackingApplicationEngine(IApplicationEngine engine,
        ConcurrentDictionary<IApplicationTask, ICountdownEvent> tasksAndCountdownEvents,
        ICountdownEvent queueModificationsCountdownEvent,
        IApplicationTask parentTask)
    {
        _engine = engine;
        _activityEngine = engine as IApplicationActivityEngine ?? throw new ArgumentException(nameof(engine),
            $"{nameof(engine)} must implement {nameof(IApplicationActivityEngine)}");
        _eventEngine = engine as IApplicationEventEngine ?? throw new ArgumentException(nameof(engine),
            $"{nameof(engine)} must implement {nameof(IApplicationActivityEngine)}");
        _tasksAndCountdownEvents = tasksAndCountdownEvents;
        _queueModificationsCountdownEvent = queueModificationsCountdownEvent;
        _parentTask = parentTask;
    }

    public IServiceProvider ServiceProvider => _engine.ServiceProvider;
    public ValueTask<bool> AreOperationsPending<T>(Func<T, bool> query) => _engine.AreOperationsPending(query);

    private void RecordChildTask(IApplicationTask childTask)
    {
        var parentCountdownEvent = _tasksAndCountdownEvents.GetOrAdd(
            _parentTask,
            new ListeningCountdownEvent(_queueModificationsCountdownEvent, 1)
        );
        _tasksAndCountdownEvents.GetOrAdd(childTask, new ChildCountdownEvent(parentCountdownEvent, 1));
        parentCountdownEvent.AddCount();
    }

    public ValueTask Dispatch(IApplicationActivity applicationActivity)
    {
        RecordChildTask(applicationActivity);

        return _activityEngine.Dispatch(applicationActivity);
    }

    public ValueTask Wait(IApplicationTask task) => _engine.Wait(task);

    public ValueTask Fire(IApplicationEvent applicationEvent)
    {
        RecordChildTask(applicationEvent);

        return _eventEngine.Fire(applicationEvent);
    }

    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent
    {
        _eventEngine.On(eventHandler);
    }
}
