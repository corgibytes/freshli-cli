using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class RestartAnalysisActivity : StartAnalysisActivityBase
{
    public RestartAnalysisActivity(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser) : base(cacheManager, historyIntervalParser)
    {
    }

    public override void Handle(IApplicationEventEngine eventClient)
    {
        HandleWithCacheFailure<UnableToRestartAnalysisEvent>(eventClient);
    }
}
