namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class RestartAnalysisActivity : StartAnalysisActivityBase<UnableToRestartAnalysisEvent>
{
    public RestartAnalysisActivity(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser) : base(
        cacheManager, historyIntervalParser)
    {
    }
}
