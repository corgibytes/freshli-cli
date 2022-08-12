using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.CacheDestroy;

public class DestroyCacheActivity : IApplicationActivity
{
    public DestroyCacheActivity(ICacheManager cacheManager, string cacheDir)
    {
        CacheManager = cacheManager;
        CacheDir = cacheDir;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public ICacheManager CacheManager { get; }
    // ReSharper disable once MemberCanBePrivate.Global
    public string CacheDir { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Destroy the cache
        try
        {
            var exitCode = CacheManager.Destroy(CacheDir).ToExitCode();
            eventClient.Fire(new CacheDestroyedEvent { ExitCode = exitCode });
        }
        catch (CacheException error)
        {
            eventClient.Fire(new CacheDestroyFailedEvent { ResultMessage = error.Message });
        }
    }
}
