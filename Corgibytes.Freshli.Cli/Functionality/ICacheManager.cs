using System;
using System.IO;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheManager
{
    public ValueTask<bool> ValidateCacheDirectory();
    public ValueTask<bool> Destroy();
    public ValueTask<DirectoryInfo> GetDirectoryInCache(params string[] directoryStructure);
    public ValueTask<bool> Prepare();
    public ValueTask<string> StoreBomInCache(string pathToBom, Guid analysisId, DateTimeOffset asOfDateTime);

    public ICacheDb GetCacheDb();
}
