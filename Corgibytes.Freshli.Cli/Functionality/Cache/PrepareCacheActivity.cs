using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class PrepareCacheActivity : IApplicationActivity
{
    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();

        try
        {
            if (await cacheManager.Prepare())
            {
                await eventClient.Fire(new CachePreparedEvent());
            }
            else
            {
                await eventClient.Fire(new CachePrepareFailedEvent
                {
                    ErrorMessage = "Failed to prepare the cache for an unknown reason"
                });
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new CachePrepareFailedEvent
            {
                ErrorMessage = error.Message,
                Exception = error
            });
        }
    }
}
