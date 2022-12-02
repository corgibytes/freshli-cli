using System;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEventEngine : IApplicationEngine
{
    public ValueTask Fire(IApplicationEvent applicationEvent);
    public void On<TEvent>(Func<TEvent, ValueTask> eventHandler) where TEvent : IApplicationEvent;
}
