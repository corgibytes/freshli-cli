using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class DestroyCacheActivity : IApplicationActivity
{
    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();

        try
        {
            var exitCode = (await cacheManager.Destroy()).ToExitCode();
            await eventClient.Fire(new CacheDestroyedEvent { ExitCode = exitCode }, cancellationToken);
        }
        catch (CacheException error)
        {
            await eventClient.Fire(new CacheDestroyFailedEvent { ResultMessage = error.Message }, cancellationToken);
        }
    }
}
