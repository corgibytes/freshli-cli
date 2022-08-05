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
        Clone(gitSource, gitPath);

        // If a branch is defined, checkout branch
        if (!string.IsNullOrEmpty(branch))
        {
            Checkout(gitSource, gitPath);
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

    private static void Checkout(GitSource gitSource, string gitPath)
    {
        try
        {
            Invoke.Command(gitPath, $"checkout {gitSource.Branch ?? ""}", gitSource.Directory.FullName);
        }
        catch (IOException e)
        {
            Delete(gitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private static void Delete(GitSource gitSource)
    {
        using var db = new CacheContext(gitSource.CacheDir);
        var entry = db.CachedGitSources.Find(gitSource.Hash);
        db.CachedGitSources.Remove(entry!);

        gitSource.Directory.Delete(true);
    }

    private static void Clone(GitSource gitSource, string gitPath)
    {
        try
        {
            Invoke.Command(gitPath, $"clone {gitSource.Url} .", gitSource.Directory.FullName);
        }
        catch (IOException e)
        {
            Delete(gitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private static string FetchCurrentBranch(GitSource gitSource, string gitPath) =>
        Invoke.Command(gitPath, "branch --show-current", gitSource.Directory.FullName).Replace("\n", "");
}
