using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.CacheDestroy;

public class DestroyCacheActivity : IApplicationActivity
{
    [JsonProperty] private readonly string _cacheDir;

    [JsonProperty] private readonly ICacheManager _cacheManager;

    public DestroyCacheActivity(ICacheManager cacheManager, string cacheDir)
    {
        _cacheManager = cacheManager;
        _cacheDir = cacheDir;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Destroy the cache
        try
        {
            var exitCode = _cacheManager.Destroy(_cacheDir).ToExitCode();
            eventClient.Fire(new CacheDestroyedEvent { ExitCode = exitCode });
        }
        catch (CacheException error)
        {
            eventClient.Fire(new CacheDestroyFailedEvent { ResultMessage = error.Message });
        }
    }
}
