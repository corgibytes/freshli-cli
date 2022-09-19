using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class StartAnalysisActivityBase<TErrorEvent> : IApplicationActivity
    where TErrorEvent : ErrorEvent, new()
{
    protected StartAnalysisActivityBase(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser)
    {
        CacheManager = cacheManager;
        HistoryIntervalParser = historyIntervalParser;
    }

    public string GitPath { get; init; } = null!;
    public string CacheDirectory { get; init; } = null!;
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }

    public ICacheManager CacheManager { get; }
    public IHistoryIntervalParser HistoryIntervalParser { get; }

    public void Handle(IApplicationEventEngine eventClient) => HandleWithCacheFailure(eventClient);

    private void FireAnalysisStartedEvent(IApplicationEventEngine eventClient)
    {
        var cacheDb = CacheManager.GetCacheDb(CacheDirectory);
        var id = cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval,
            UseCommitHistory));
        eventClient.Fire(new AnalysisStartedEvent
        {
            AnalysisId = id,
            CacheDir = CacheDirectory,
            GitPath = GitPath
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
        if (!CacheManager.ValidateDirIsCache(CacheDirectory))
        {
            eventClient.Fire(CreateErrorEvent());
            return true;
        }

        return false;
    }

    protected virtual TErrorEvent CreateErrorEvent() =>
        // ReSharper disable once UseStringInterpolation
        new() { ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.", CacheDirectory) };

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
