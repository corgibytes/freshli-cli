using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class PrepareCacheActivity : IApplicationActivity
{
    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();

        try
        {
            if (cacheManager.Prepare())
            {
                eventClient.Fire(new CachePreparedEvent());
            }
            else
            {
                eventClient.Fire(new CachePrepareFailedEvent
                {
                    ErrorMessage = "Failed to prepare the cache for an unknown reason"
                });
            }
        }
        catch (Exception error)
        {
            eventClient.Fire(new CachePrepareFailedEvent { ErrorMessage = error.Message });
        }
    }
}
