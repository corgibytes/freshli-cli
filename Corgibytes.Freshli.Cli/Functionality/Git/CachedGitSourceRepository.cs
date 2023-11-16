using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CachedGitSourceRepository : ICachedGitSourceRepository
{
    private readonly ICommandInvoker _commandInvoker;

    public CachedGitSourceRepository(ICommandInvoker commandInvoker, IConfiguration configuration,
        ICacheManager cacheManager)
    {
        _commandInvoker = commandInvoker;
        Configuration = configuration;
        CacheManager = cacheManager;
    }

    private ICacheManager CacheManager { get; }
    private IConfiguration Configuration { get; }

    public async ValueTask<CachedGitSource> FindOneByRepositoryId(string repositoryId)
    {
        var cacheDb = await CacheManager.GetCacheDb();
        var entry = await cacheDb.RetrieveCachedGitSource(new CachedGitSourceId(repositoryId));
        _ = entry ?? throw new CacheException(CliOutput.CachedGitSourceRepository_No_Repository_Found_In_Cache);

        return entry;
    }

    public async ValueTask Save(CachedGitSource cachedGitSource)
    {
        var cacheDb = await CacheManager.GetCacheDb();
        await cacheDb.AddCachedGitSource(cachedGitSource);
    }

    public async ValueTask<CachedGitSource> CloneOrPull(string url, string? branch)
    {
        // Generate a unique repositoryId for the repository based on its URL and branch.
        var id = new CachedGitSourceId(url, branch);

        var cacheDb = await CacheManager.GetCacheDb();
        var existingCachedGitSource = await cacheDb.RetrieveCachedGitSource(id);
        if (existingCachedGitSource is not null)
        {
            await Pull(existingCachedGitSource);
            return existingCachedGitSource;
        }

        var directory = await CacheManager.GetDirectoryInCache("repositories", id.Id);

        var cachedGitSource = new CachedGitSource
        {
            Id = id.Id,
            Url = url,
            Branch = branch,
            LocalPath = directory.FullName
        };

        // If not yet cloned, clone from URL.
        await Clone(cachedGitSource);

        // If a branch is defined, checkout branch
        if (!string.IsNullOrEmpty(branch))
        {
            await Checkout(cachedGitSource);
        }
        else
        {
            cachedGitSource.Branch = await FetchCurrentBranch(cachedGitSource);
        }

        await cacheDb.AddCachedGitSource(cachedGitSource);

        return cachedGitSource;
    }

    private async ValueTask Pull(CachedGitSource cachedGitSource)
    {
        var branch = cachedGitSource.Branch;
        string? commandOutput = null;

        try
        {
            var rawCommandOutput = await _commandInvoker.Run(
                Configuration.GitPath, $"pull origin {branch ?? ""}",
                cachedGitSource.LocalPath
            );
            commandOutput = rawCommandOutput.Replace("\n", " ");
        }
        catch (IOException e)
        {
            if (commandOutput == "Already up to date.")
            {
                throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
            }
        }
    }

    private async ValueTask Checkout(CachedGitSource cachedGitSource)
    {
        try
        {
            await _commandInvoker.Run(Configuration.GitPath, $"checkout {cachedGitSource.Branch ?? ""}",
                cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            await Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private async ValueTask Delete(CachedGitSource cachedGitSource)
    {
        var directory = new DirectoryInfo(cachedGitSource.LocalPath);
        await using var db = new CacheContext(Configuration.CacheDir);
        var cacheDb = await CacheManager.GetCacheDb();
        var entry = await cacheDb.RetrieveCachedGitSource(new CachedGitSourceId(cachedGitSource.Id));
        if (entry != null)
        {
            await cacheDb.RemoveCachedGitSource(entry);
        }

        await Task.Run(() => directory.Delete(true));
    }

    private async ValueTask Clone(CachedGitSource cachedGitSource)
    {
        try
        {
            await _commandInvoker.Run(
                Configuration.GitPath, $"clone {cachedGitSource.Url} .", cachedGitSource.LocalPath);
        }
        catch (IOException e)
        {
            await Delete(cachedGitSource);
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private async ValueTask<string> FetchCurrentBranch(CachedGitSource cachedGitSource)
    {
        var rawCommandOutput = await _commandInvoker.Run(
            Configuration.GitPath, "branch --show-current", cachedGitSource.LocalPath);
        return rawCommandOutput.Replace("\n", "");
    }
}
