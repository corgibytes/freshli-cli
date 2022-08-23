using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Resources;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CachedGitSourceRepository : ICachedGitSourceRepository
{
    public CachedGitSourceRepository(ICacheManager cacheManager) =>
        CacheManager = cacheManager;

    [JsonProperty] private ICacheManager CacheManager { get; }

    public CachedGitSource FindOneByHash(string hash, string cacheDir)
    {
        using var db = new CacheContext(cacheDir);
        var entry = db.CachedGitSources.Find(hash);
        if (entry == null)
        {
            throw new CacheException(CliOutput.CachedGitSourceRepository_No_Repository_Found_In_Cache);
        }

        return entry;
    }

    public CachedGitSource CloneOrPull(string url, string? branch, string cacheDir, string gitPath)
    {
        // Ensure the cache directory is ready for use.
        CacheManager.Prepare(cacheDir);

        // Generate a unique hash for the repository based on its URL and branch.
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url + branch));
        var stringBuilder = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            stringBuilder.Append(hashByte.ToString("x2"));
        }

        var hash = stringBuilder.ToString();

        using var db = new CacheContext(cacheDir);
        var existingCachedGitSource = db.CachedGitSources.Find(hash);
        if (existingCachedGitSource is not null)
        {
            return db.CachedGitSources.Find(hash) ?? throw new InvalidOperationException();
        }

        var directory = CacheManager.GetDirectoryInCache(cacheDir, new[] { "repositories", hash });

        var cachedGitSource = new CachedGitSource(hash, url, branch, directory.FullName);
        db.CachedGitSources.Add(cachedGitSource);
        db.SaveChanges();

        if (directory.GetFiles().Any() || directory.GetDirectories().Any())
        {
            Pull(cachedGitSource, gitPath);
            return cachedGitSource;
        }

        // If not yet cloned, clone from URL.
        Clone(cachedGitSource, gitPath);

        // If a branch is defined, checkout branch
        if (!string.IsNullOrEmpty(branch))
        {
            Checkout(cachedGitSource, gitPath);
        }

        return cachedGitSource;
    }

    private static void Pull(CachedGitSource cachedGitSource, string gitPath)
    {
        var branch = cachedGitSource.Branch;
        if (cachedGitSource.Branch == null)
        {
            branch = FetchCurrentBranch(cachedGitSource, gitPath);
        }

        string? commandOutput = null;

        try
        {
            commandOutput = Invoke.Command(gitPath, $"pull origin {branch ?? ""}", cachedGitSource.LocalPath)
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

    private static void Checkout(CachedGitSource cachedGitSource, string gitPath)
    {
        try
        {
            Invoke.Command(gitPath, $"checkout {cachedGitSource.Branch ?? ""}", cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private static void Delete(CachedGitSource cachedGitSource)
    {
        var directory = new DirectoryInfo(cachedGitSource.LocalPath);
        using var db = new CacheContext(directory.FullName);
        var entry = db.CachedGitSources.Find(cachedGitSource.Id);
        db.CachedGitSources.Remove(entry!);

        directory.Delete(true);
    }

    private static void Clone(CachedGitSource cachedGitSource, string gitPath)
    {
        try
        {
            Invoke.Command(gitPath, $"clone {cachedGitSource.Url} .", cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private static string FetchCurrentBranch(CachedGitSource cachedGitSource, string gitPath) =>
        Invoke.Command(gitPath, "branch --show-current", cachedGitSource.LocalPath).Replace("\n", "");
}
