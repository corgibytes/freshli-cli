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

    protected IConfiguration Configuration { get; set; } = null!;
    private ICacheManager CacheManager { get; set; } = null!;
    private IHistoryIntervalParser HistoryIntervalParser { get; set; } = null!;

    public void Handle(IApplicationEventEngine eventClient)
    {
        Configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        CacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        HistoryIntervalParser = eventClient.ServiceProvider.GetRequiredService<IHistoryIntervalParser>();

        HandleWithCacheFailure(eventClient);
    }

    private void FireAnalysisStartedEvent(IApplicationEventEngine eventClient)
    {
        var cacheDb = CacheManager.GetCacheDb();
        var id = cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval,
            UseCommitHistory));
        eventClient.Fire(new AnalysisStartedEvent
        {
            AnalysisId = id
        });
    }

    private bool FireInvalidHistoryEventIfNeeded(IApplicationEventEngine eventClient)
    {
        if (!HistoryIntervalParser.IsValid(HistoryInterval))
        {
            eventClient.Fire(new InvalidHistoryIntervalEvent
            {
                // ReSharper disable once UseStringInterpolation
                ErrorMessage = string.Format("The value '{0}' is not a valid history interval.", HistoryInterval)
            });
            return true;
        }

        return false;
    }

    private bool FireCacheNotFoundEventIfNeeded(IApplicationEventEngine eventClient)
    {
        if (!CacheManager.ValidateDirIsCache(Configuration.CacheDir))
        {
            eventClient.Fire(CreateErrorEvent());
            return true;
        }

        return false;
    }

    protected virtual TErrorEvent CreateErrorEvent() =>
        // ReSharper disable once UseStringInterpolation
        new() { ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.", Configuration.CacheDir) };

    private void HandleWithCacheFailure(IApplicationEventEngine eventClient)
    {
        if (FireCacheNotFoundEventIfNeeded(eventClient))
        {
            return;
        }

        if (FireInvalidHistoryEventIfNeeded(eventClient))
        {
            return;
        }

        FireAnalysisStartedEvent(eventClient);
    }
}
