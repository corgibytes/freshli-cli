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
    private readonly ConcurrentDictionary<IApplicationTask, ApplicationTaskWaitToken> _tasksAndWaitInfos;

    public ChildTrackingApplicationEngine(IApplicationEngine engine,
        ConcurrentDictionary<IApplicationTask, ApplicationTaskWaitToken> tasksAndWaitInfos,
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
        _tasksAndWaitInfos = tasksAndWaitInfos;
        _parentTask = parentTask;
    }

    public IServiceProvider ServiceProvider => _engine.ServiceProvider;

    private async Task RecordChildTask(IApplicationTask childTask, CancellationToken cancellationToken)
    {
        ApplicationTaskWaitToken? parentTaskWaitInfo;
        while (!_tasksAndWaitInfos.TryGetValue(_parentTask, out parentTaskWaitInfo))
        {
            await Task.Delay(TimeSpan.FromMicroseconds(10), cancellationToken);
        }

        var childTaskWaitInfo = new ApplicationTaskWaitToken();
        if (!_tasksAndWaitInfos.TryAdd(childTask, childTaskWaitInfo))
        {
            throw new Exception("Failed to add child event");
        }
        parentTaskWaitInfo.AddChildResetEvent(childTaskWaitInfo);
    }

    public async ValueTask Dispatch(IApplicationActivity applicationActivity, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        if (mode == ApplicationTaskMode.Tracked)
        {
            await RecordChildTask(applicationActivity, cancellationToken);
        }

        await _activityEngine.Dispatch(applicationActivity, cancellationToken, mode);
    }

    public ValueTask Wait(IApplicationTask task, CancellationToken cancellationToken, ApplicationTaskWaitToken? excluding = null) => _engine.Wait(task, cancellationToken, excluding);

    public async ValueTask RegisterChildWaitToken(IApplicationTask task, ApplicationTaskWaitToken waitToken,
        CancellationToken cancellationToken) =>
        await _engine.RegisterChildWaitToken(task, waitToken, cancellationToken);

    public async ValueTask Fire(IApplicationEvent applicationEvent, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked)
    {
        if (mode == ApplicationTaskMode.Tracked)
        {
            await RecordChildTask(applicationEvent, cancellationToken);
        }

        await _eventEngine.Fire(applicationEvent, cancellationToken, mode);
    }

    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent
    {
        _eventEngine.On(eventHandler);
    }
}
