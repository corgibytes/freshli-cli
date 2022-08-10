using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.CacheDestroy;

public class CacheDestroyFailedEvent : IApplicationEvent
{
    public string ResultMessage { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
