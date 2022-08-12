using System.IO;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash, DirectoryInfo cacheDir);

    public CachedGitSource CloneOrPull(string url, string? branch, DirectoryInfo cacheDir, string gitPath);
}
