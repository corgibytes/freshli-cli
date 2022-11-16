using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class StartAnalysisActivityBase<TErrorEvent> : IApplicationActivity
    where TErrorEvent : ErrorEvent, new()
{
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }
    public RevisionHistoryMode RevisionHistoryMode { get; init; }

    protected IConfiguration Configuration { get; private set; } = null!;
    private ICacheManager CacheManager { get; set; } = null!;
    private IHistoryIntervalParser HistoryIntervalParser { get; set; } = null!;

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        Configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        CacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        HistoryIntervalParser = eventClient.ServiceProvider.GetRequiredService<IHistoryIntervalParser>();

        await HandleWithCacheFailure(eventClient);
    }

    private async ValueTask FireAnalysisStartedEvent(IApplicationEventEngine eventClient)
    {
        var cacheDb = CacheManager.GetCacheDb();
        var id = cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval,
            UseCommitHistory, RevisionHistoryMode));
        await eventClient.Fire(new AnalysisStartedEvent { AnalysisId = id });
    }

    private async ValueTask<bool> FireInvalidHistoryEventIfNeeded(IApplicationEventEngine eventClient)
    {
        if (!HistoryIntervalParser.IsValid(HistoryInterval))
        {
            await eventClient.Fire(new InvalidHistoryIntervalEvent
            {
                // ReSharper disable once UseStringInterpolation
                ErrorMessage = string.Format("The value '{0}' is not a valid history interval.", HistoryInterval)
            });
            return true;
        }

        return false;
    }

    private async ValueTask<bool> FireCacheNotFoundEventIfNeeded(IApplicationEventEngine eventClient)
    {
        if (!CacheManager.ValidateCacheDirectory())
        {
            await eventClient.Fire(CreateErrorEvent());
            return true;
        }

        return false;
    }

    protected virtual TErrorEvent CreateErrorEvent() =>
        // ReSharper disable once UseStringInterpolation
        new()
        {
            ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.",
                Configuration.CacheDir)
        };

    private async ValueTask HandleWithCacheFailure(IApplicationEventEngine eventClient)
    {
        if (await FireCacheNotFoundEventIfNeeded(eventClient))
        {
            return;
        }

        if (await FireInvalidHistoryEventIfNeeded(eventClient))
        {
            return;
        }

        await FireAnalysisStartedEvent(eventClient);
    }
}
