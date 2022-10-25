using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Resources;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CachedGitSourceRepository : ICachedGitSourceRepository
{
    [JsonProperty] private readonly ICommandInvoker _commandInvoker;

    public CachedGitSourceRepository(ICommandInvoker commandInvoker, IConfiguration configuration,
        ICacheManager cacheManager)
    {
        _commandInvoker = commandInvoker;
        Configuration = configuration;
        CacheManager = cacheManager;
    }

    [JsonProperty] private ICacheManager CacheManager { get; }
    [JsonProperty] private IConfiguration Configuration { get; }

    public CachedGitSource FindOneByRepositoryId(string repositoryId)
    {
        var entry = CacheManager.GetCacheDb().RetrieveCachedGitSource(new CachedGitSourceId(repositoryId));
        if (entry == null)
        {
            throw new CacheException(CliOutput.CachedGitSourceRepository_No_Repository_Found_In_Cache);
        }

        return entry;
    }

    public void Save(CachedGitSource cachedGitSource)
    {
        CacheManager.GetCacheDb().AddCachedGitSource(cachedGitSource);
    }

    public CachedGitSource CloneOrPull(string url, string? branch)
    {
        // Generate a unique repositoryId for the repository based on its URL and branch.
        var id = new CachedGitSourceId(url, branch);

        var existingCachedGitSource = CacheManager.GetCacheDb().RetrieveCachedGitSource(id);
        if (existingCachedGitSource is not null)
        {
            return existingCachedGitSource;
        }

        var directory = CacheManager.GetDirectoryInCache("repositories", id.Id);

        var cachedGitSource = new CachedGitSource(id.Id, url, branch, directory.FullName);
        CacheManager.GetCacheDb().AddCachedGitSource(cachedGitSource);

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
            commandOutput = _commandInvoker.Run(Configuration.GitPath, $"pull origin {branch ?? ""}",
                    cachedGitSource.LocalPath)
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
            _commandInvoker.Run(Configuration.GitPath, $"checkout {cachedGitSource.Branch ?? ""}",
                cachedGitSource.LocalPath);
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
        var entry = CacheManager.GetCacheDb().RetrieveCachedGitSource(new CachedGitSourceId(cachedGitSource.Id));
        if (entry != null)
        {
            CacheManager.GetCacheDb().RemoveCachedGitSource(entry);
        }

        directory.Delete(true);
    }

    private void Clone(CachedGitSource cachedGitSource)
    {
        try
        {
            _commandInvoker.Run(Configuration.GitPath, $"clone {cachedGitSource.Url} .", cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private string FetchCurrentBranch(CachedGitSource cachedGitSource) =>
        _commandInvoker.Run(Configuration.GitPath, "branch --show-current", cachedGitSource.LocalPath)
            .Replace("\n", "");
}
