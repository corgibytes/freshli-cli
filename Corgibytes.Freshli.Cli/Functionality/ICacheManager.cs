using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheManager
{
    public bool ValidateDirIsCache(string cacheDir);
    public bool Prepare(string cacheDir);
    public bool Destroy(string cacheDir);
    public DirectoryInfo GetDirectoryInCache(string cacheDirPath, string[] directoryStructure);

    public ICacheDb GetCacheDb(string cacheDir);
}
