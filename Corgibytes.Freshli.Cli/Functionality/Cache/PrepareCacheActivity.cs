using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class PrepareCacheActivity : IApplicationActivity
{
    public void Handle(IApplicationEventEngine eventClient)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        Console.Out.WriteLine(CliOutput.CachePrepareCommandRunner_Run_Preparing_cache, configuration.CacheDir);
        try
        {
            var cacheManager = new CacheManager(configuration);
            cacheManager.Prepare();
            eventClient.Fire(new CachePreparedEvent());
        }
        catch (CacheException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }
}
