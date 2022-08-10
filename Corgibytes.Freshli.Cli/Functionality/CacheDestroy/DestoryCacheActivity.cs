using System.IO;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.CacheDestroy;

public class DestroyCacheActivity : IApplicationActivity
{
    // ReSharper disable once MemberCanBePrivate.Global
    public string CacheDir { get; }

    public DestroyCacheActivity(string cacheDir)
    {
        CacheDir = cacheDir;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Destroy the cache
        try
        {
            var exitCode = Cache.Destroy(new DirectoryInfo(CacheDir)).ToExitCode();
            eventClient.Fire(new CacheDestroyedEvent() {ExitCode = exitCode});
        }
        catch (CacheException error)
        {
            eventClient.Fire(new CacheDestroyFailedEvent() {ResultMessage = error.Message});
        }
    }
}
