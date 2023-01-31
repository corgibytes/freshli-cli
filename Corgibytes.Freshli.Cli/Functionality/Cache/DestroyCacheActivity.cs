using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class DestroyCacheActivity : IApplicationActivity
{
    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();

        try
        {
            var exitCode = (await cacheManager.Destroy()).ToExitCode();
            await eventClient.Fire(new CacheDestroyedEvent { ExitCode = exitCode });
        }
        catch (CacheException error)
        {
            await eventClient.Fire(new CacheDestroyFailedEvent { ResultMessage = error.Message });
        }
    }
}
