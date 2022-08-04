using Corgibytes.Freshli.Cli.Functionality.Engine;

// ReSharper disable UseStringInterpolation

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class StartAnalysisActivity : StartAnalysisActivityBase
{
    public StartAnalysisActivity(ICacheManager cacheManager, IHistoryIntervalParser historyIntervalParser) : base(cacheManager, historyIntervalParser)
    {
    }

    public override void Handle(IApplicationEventEngine eventClient)
    {
        HandleWithCacheFailure<CacheWasNotPreparedEvent>(eventClient);
    }
}
