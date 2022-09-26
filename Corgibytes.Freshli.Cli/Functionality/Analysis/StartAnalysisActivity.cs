namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class StartAnalysisActivity : StartAnalysisActivityBase<CacheWasNotPreparedEvent>
{
    public StartAnalysisActivity(IConfiguration configuration, ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser) : base(
        configuration, cacheManager, historyIntervalParser)
    {
    }

    protected override CacheWasNotPreparedEvent CreateErrorEvent() =>
        new()
        {
            // TODO: Translate this string
            // ReSharper disable UseStringInterpolation
            ErrorMessage = string.Format("Unable to locate a valid cache directory at: '{0}'.", Configuration.CacheDir),
            RepositoryUrl = RepositoryUrl,
            RepositoryBranch = RepositoryBranch,
            HistoryInterval = HistoryInterval,
            UseCommitHistory = UseCommitHistory
        };
}
