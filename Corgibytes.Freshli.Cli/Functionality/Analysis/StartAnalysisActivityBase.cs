using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class StartAnalysisActivityBase : IApplicationActivity
{
    public StartAnalysisActivityBase(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser)
    {
        CacheManager = cacheManager;
        HistoryIntervalParser = historyIntervalParser;
    }

    public string CacheDirectory { get; init; } = null!;
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    protected ICacheManager CacheManager { get; }
    protected IHistoryIntervalParser HistoryIntervalParser { get; }
    public abstract void Handle(IApplicationEventEngine eventClient);

    protected void FireAnalysisStartedEvent(IApplicationEventEngine eventClient)
    {
        var cacheDb = CacheManager.GetCacheDb(CacheDirectory);
        var id = cacheDb.SaveAnalysis(new(RepositoryUrl, RepositoryBranch, HistoryInterval));
        eventClient.Fire(new AnalysisStartedEvent {AnalysisId = id});
    }

    protected bool FireInvalidHistoryEventIfNeeded(IApplicationEventEngine eventClient)
    {
        if (!HistoryIntervalParser.IsValid(HistoryInterval))
        {
            eventClient.Fire(new InvalidHistoryIntervalEvent
            {
                ErrorMessage = string.Format("The value '{0}' is not a valid history interval.", HistoryInterval)
            });
            return true;
        }

        return false;
    }

    protected bool FireCacheNotFoundEventIfNeeded<TFailureEvent>(IApplicationEventEngine eventClient) where TFailureEvent : FailureEvent, new()
    {
        if (!CacheManager.ValidateDirIsCache(CacheDirectory))
        {
            eventClient.Fire(new TFailureEvent
            {
                ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.", CacheDirectory)
            });
            return true;
        }

        return false;
    }

    protected void HandleWithCacheFailure<TFailureEvent>(IApplicationEventEngine eventClient) where TFailureEvent : FailureEvent, new()
    {
        if (FireCacheNotFoundEventIfNeeded<TFailureEvent>(eventClient))
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
