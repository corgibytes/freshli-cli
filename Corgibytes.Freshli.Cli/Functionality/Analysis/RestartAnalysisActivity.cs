namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class RestartAnalysisActivity : StartAnalysisActivityBase<UnableToRestartAnalysisEvent>
{
    public RestartAnalysisActivity(IConfiguration configuration, ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser) : base(
        configuration, cacheManager, historyIntervalParser)
    {
    }
}
