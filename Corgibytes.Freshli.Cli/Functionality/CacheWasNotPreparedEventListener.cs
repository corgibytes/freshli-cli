using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheWasNotPreparedEventListener : IApplicationActivity
{
    public CacheWasNotPreparedEventListener(PrepareCacheMessage prepareCacheMessage)
    {

    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        eventClient

    }
}
