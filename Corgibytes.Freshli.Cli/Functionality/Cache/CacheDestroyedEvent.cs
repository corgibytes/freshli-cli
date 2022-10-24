using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class CacheDestroyedEvent : IApplicationEvent
{
    public int ExitCode { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
