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
    public CachedGitSourceRepository(IConfiguration configuration, ICacheManager cacheManager)
    {
        Configuration = configuration;
        CacheManager = cacheManager;
    }

    [JsonProperty] private ICacheManager CacheManager { get; }
    [JsonProperty] private IConfiguration Configuration { get; }

    public CachedGitSource FindOneByHash(string hash)
    {
        using var db = new CacheContext(Configuration.CacheDir);
        var entry = db.CachedGitSources.Find(hash);
        if (entry == null)
        {
            throw new CacheException(CliOutput.CachedGitSourceRepository_No_Repository_Found_In_Cache);
        }

        return entry;
    }

    public CachedGitSource CloneOrPull(string url, string? branch)
    {
        // Ensure the cache directory is ready for use.
        CacheManager.Prepare(Configuration.CacheDir);

        // Generate a unique hash for the repository based on its URL and branch.
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url + branch));
        var stringBuilder = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            stringBuilder.Append(hashByte.ToString("x2"));
        }

        var hash = stringBuilder.ToString();

        using var db = new CacheContext(Configuration.CacheDir);
        var existingCachedGitSource = db.CachedGitSources.Find(hash);
        if (existingCachedGitSource is not null)
        {
            return existingCachedGitSource;
        }

        var directory = CacheManager.GetDirectoryInCache(Configuration.CacheDir, new[] { "repositories", hash });

        var cachedGitSource = new CachedGitSource(hash, url, branch, directory.FullName);
        db.CachedGitSources.Add(cachedGitSource);
        db.SaveChanges();

        if (directory.GetFiles().Any() || directory.GetDirectories().Any())
        {
            Pull(cachedGitSource);
            return cachedGitSource;
        }

        // If not yet cloned, clone from URL.
        Clone(cachedGitSource);

        // If a branch is defined, checkout branch
        if (!string.IsNullOrEmpty(branch))
        {
            Checkout(cachedGitSource);
        }

        return cachedGitSource;
    }

    private void Pull(CachedGitSource cachedGitSource)
    {
        var branch = cachedGitSource.Branch;
        if (cachedGitSource.Branch == null)
        {
            branch = FetchCurrentBranch(cachedGitSource);
        }

        string? commandOutput = null;

        try
        {
            commandOutput = Invoke.Command(Configuration.GitPath, $"pull origin {branch ?? ""}", cachedGitSource.LocalPath)
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

    private void Checkout(CachedGitSource cachedGitSource)
    {
        try
        {
            Invoke.Command(Configuration.GitPath, $"checkout {cachedGitSource.Branch ?? ""}", cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private void Delete(CachedGitSource cachedGitSource)
    {
        var directory = new DirectoryInfo(cachedGitSource.LocalPath);
        using var db = new CacheContext(Configuration.CacheDir);
        var entry = db.CachedGitSources.Find(cachedGitSource.Id);
        db.CachedGitSources.Remove(entry!);

        directory.Delete(true);
    }

    private void Clone(CachedGitSource cachedGitSource)
    {
        try
        {
            Invoke.Command(Configuration.GitPath, $"clone {cachedGitSource.Url} .", cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private string FetchCurrentBranch(CachedGitSource cachedGitSource) =>
        Invoke.Command(Configuration.GitPath, "branch --show-current", cachedGitSource.LocalPath).Replace("\n", "");
}
