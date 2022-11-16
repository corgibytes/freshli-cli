using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheManager
{
    // TODO: Make this method return ValueTask<bool>
    public bool ValidateCacheDirectory();
    // TODO: Make this method return ValueTask<bool>
    public bool Destroy();
    // TODO: Make this method return ValueTask<DirectoryInfo>
    public DirectoryInfo GetDirectoryInCache(params string[] directoryStructure);
    // TODO: Make this method return ValueTask<bool>
    public bool Prepare();
    // TODO: Make this method return ValueTask<string>
    public string StoreBomInCache(string pathToBom, Guid analysisId, DateTimeOffset asOfDateTime);

    public ICacheDb GetCacheDb();
}
