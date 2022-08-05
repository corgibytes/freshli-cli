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
            Pull(gitSource, gitPath);
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

    private static void Pull(GitSource gitSource, string gitPath)
    {
        var branch = gitSource.Branch;
        if (gitSource.Branch == null)
        {
            branch = FetchCurrentBranch(gitSource, gitPath);
        }

        string? commandOutput = null;

        try
        {
            commandOutput = Invoke.Command(gitPath, $"pull origin {branch ?? ""}", gitSource.Directory.FullName)
                .Replace("\n", " ");
        }
        catch (IOException e)
        {
            if (commandOutput == "Already up to date.")
            {
                throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
            }
        }
    }

    private static string FetchCurrentBranch(GitSource gitSource, string gitPath) =>
        Invoke.Command(gitPath, "branch --show-current", gitSource.Directory.FullName).Replace("\n", "");
}
