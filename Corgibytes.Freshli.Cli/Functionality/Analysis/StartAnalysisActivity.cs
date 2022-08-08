using Corgibytes.Freshli.Cli.Functionality.Engine;

// ReSharper disable UseStringInterpolation

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class StartAnalysisActivity : StartAnalysisActivityBase<CacheWasNotPreparedEvent>
{
    public StartAnalysisActivity(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser) : base(cacheManager, historyIntervalParser)
    {
    }

    protected override CacheWasNotPreparedEvent CreateErrorEvent()
    {
        return new()
        {
            ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.", CacheDirectory),
            CacheDirectory = CacheDirectory,
            RepositoryUrl = RepositoryUrl,
            RepositoryBranch = RepositoryBranch,
            HistoryInterval = HistoryInterval
        };
    }
}
