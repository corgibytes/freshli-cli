using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api.Auth;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public interface ICacheManager
{
    public ValueTask<bool> ValidateCacheDirectory();
    public ValueTask<bool> Destroy();
    public ValueTask<DirectoryInfo> GetDirectoryInCache(params string[] directoryStructure);
    public ValueTask<bool> Prepare();
    public ValueTask<string> StoreBomInCache(string pathToBom, Guid analysisId, DateTimeOffset asOfDateTime, string pathToManifest);
    public ValueTask<string> StoreApiCredentials(ApiCredentials credentials);
    // ReSharper disable once UnusedMember.Global
    public ValueTask<ApiCredentials?> GetApiCredentials();

    public ValueTask<ICacheDb> GetCacheDb();
}
