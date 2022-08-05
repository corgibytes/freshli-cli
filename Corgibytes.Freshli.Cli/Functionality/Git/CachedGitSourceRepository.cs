using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CachedGitSourceRepository : ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash, DirectoryInfo cacheDir)
    {
        using var db = new CacheContext(cacheDir);
        var entry = db.CachedGitSources.Find(hash);
        if (entry == null)
        {
            throw new CacheException(CliOutput.CachedGitSourceRepository_No_Repository_Found_In_Cache);
        }

        return entry;
    }

    public GitSource CloneOrPull(string url, string? branch, DirectoryInfo cacheDir, string gitPath)
    {
        var gitSource = new GitSource(url, branch, cacheDir);
        if (gitSource.Cloned)
        {
            gitSource.Pull(gitPath);
            return gitSource;
        }

        // If not yet cloned, clone from URL.
        gitSource.Clone(gitPath);

        // If a branch is defined, checkout branch
        if (!string.IsNullOrEmpty(branch))
        {
            gitSource.Checkout(gitPath);
        }

        return gitSource;
    }
}
