using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Resources;

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
        _activityEngine = engine as IApplicationActivityEngine ?? throw new ArgumentException(
            string.Format(
                CliOutput.ChildTrackingApplicationEngine_ChildTrackingApplicationEngine_must_implement__0_,
                nameof(IApplicationActivityEngine)
            ),
            nameof(engine)
        );
        _eventEngine = engine as IApplicationEventEngine ?? throw new ArgumentException(
            string.Format(
                CliOutput.ChildTrackingApplicationEngine_ChildTrackingApplicationEngine_must_implement__0_,
                nameof(IApplicationActivityEngine)
            ),
            nameof(engine)
        );
        _tasksAndCountdownEvents = tasksAndCountdownEvents;
        _queueModificationsCountdownEvent = queueModificationsCountdownEvent;
        _parentTask = parentTask;
    }

    public IServiceProvider ServiceProvider => _engine.ServiceProvider;

    private void RecordChildTask(IApplicationTask childTask)
    {
        var parentCountdownEvent = _tasksAndCountdownEvents.GetOrAdd(
            _parentTask,
            new ListeningCountdownEvent(_queueModificationsCountdownEvent, 1)
        );
        _tasksAndCountdownEvents.GetOrAdd(childTask, new ChildCountdownEvent(parentCountdownEvent, 1));
        parentCountdownEvent.AddCount();
    }

    public ValueTask Dispatch(IApplicationActivity applicationActivity, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        if (mode == ApplicationTaskMode.Tracked)
        {
            RecordChildTask(applicationActivity);
        }

        return _activityEngine.Dispatch(applicationActivity, cancellationToken, mode);
    }

    public ValueTask Wait(IApplicationTask task, CancellationToken cancellationToken) => _engine.Wait(task, cancellationToken);

    public ValueTask Fire(IApplicationEvent applicationEvent, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        if (mode == ApplicationTaskMode.Tracked)
        {
            RecordChildTask(applicationEvent);
        }

        return _eventEngine.Fire(applicationEvent, cancellationToken, mode);
    }

    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent
    {
        _eventEngine.On(eventHandler);
    }
}
