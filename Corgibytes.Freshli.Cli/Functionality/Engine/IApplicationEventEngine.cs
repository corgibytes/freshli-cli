using System;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEventEngine
{
    public void Fire(IApplicationEvent applicationEvent);
    public void On<TEvent>(Action<TEvent> eventHandler) where TEvent : IApplicationEvent;
}
