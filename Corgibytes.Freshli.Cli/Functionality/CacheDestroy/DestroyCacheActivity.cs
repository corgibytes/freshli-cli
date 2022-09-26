using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.CacheDestroy;

public class DestroyCacheActivity : IApplicationActivity
{
    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        // Destroy the cache
        try
        {
            var exitCode = cacheManager.Destroy(configuration.CacheDir).ToExitCode();
            eventClient.Fire(new CacheDestroyedEvent { ExitCode = exitCode });
        }
        catch (CacheException error)
        {
            eventClient.Fire(new CacheDestroyFailedEvent { ResultMessage = error.Message });
        }
    }
}
