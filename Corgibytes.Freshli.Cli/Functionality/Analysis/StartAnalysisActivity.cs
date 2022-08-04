using Corgibytes.Freshli.Cli.Functionality.Engine;

// ReSharper disable UseStringInterpolation

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class StartAnalysisActivity : IApplicationActivity
{
    public StartAnalysisActivity(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser)
    {
        CacheManager = cacheManager;
        HistoryIntervalParser = historyIntervalParser;
    }

    public string CacheDirectory { get; init; } = null!;
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;

    private ICacheManager CacheManager { get; }
    private IHistoryIntervalParser HistoryIntervalParser { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        if (!CacheManager.ValidateDirIsCache(CacheDirectory))
        {
            eventClient.Fire(new CacheWasNotPreparedEvent
            {
                ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.", CacheDirectory)
            });
            return;
        }

        if (!HistoryIntervalParser.IsValid(HistoryInterval))
        {
            eventClient.Fire(new InvalidHistoryIntervalEvent
            {
                ErrorMessage = string.Format("The value '{0}' is not a valid history interval.", HistoryInterval)
            });
            return;
        }

        var cacheDb = CacheManager.GetCacheDb(CacheDirectory);
        var id = cacheDb.SaveAnalysis(new(RepositoryUrl, RepositoryBranch, HistoryInterval));
        eventClient.Fire(new AnalysisStartedEvent { AnalysisId = id });
    }
}
