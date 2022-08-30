using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash, string cacheDir);

    public CachedGitSource CloneOrPull(string url, string? branch, string cacheDir, string gitPath);
}
