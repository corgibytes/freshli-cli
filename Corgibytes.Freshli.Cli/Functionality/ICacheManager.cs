using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheManager
{
    public bool ValidateCacheDirectory();
    public bool Prepare();
    public bool Destroy();
    public DirectoryInfo GetDirectoryInCache(string[] directoryStructure);

    public ICacheDb GetCacheDb();
}
