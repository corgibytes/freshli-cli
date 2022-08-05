using System.IO;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash, DirectoryInfo cacheDir);

    public GitSource CloneOrPull(string url, string? branch, DirectoryInfo cacheDir, string gitPath);
}
