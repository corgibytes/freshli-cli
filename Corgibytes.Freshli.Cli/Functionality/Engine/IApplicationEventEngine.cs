using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEventEngine : IApplicationEngine
{
    public ValueTask Fire(IApplicationEvent applicationEvent, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked);
    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent;
}
