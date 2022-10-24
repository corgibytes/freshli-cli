using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheManager
{
    public bool ValidateCacheDirectory();
    public bool Prepare();
    public bool Destroy();
    public DirectoryInfo GetDirectoryInCache(params string[] directoryStructure);

    public string StoreBomInCache(string pathToBom, Guid analysisId, DateTimeOffset asOfDateTime);

    public ICacheDb GetCacheDb();
}
