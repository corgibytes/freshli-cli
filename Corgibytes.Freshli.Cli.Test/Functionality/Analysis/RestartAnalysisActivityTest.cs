using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
// ReSharper disable once UnusedType.Global
public class RestartAnalysisActivityTest :
    StartAnalysisActivityTestBase<RestartAnalysisActivity, UnableToRestartAnalysisEvent>
{
    protected override RestartAnalysisActivity Activity => new(Configuration.Object, CacheManager.Object, IntervalParser.Object)
    {
        RepositoryUrl = "http://git.example.com",
        RepositoryBranch = "main",
        HistoryInterval = "1m"
    };
}
