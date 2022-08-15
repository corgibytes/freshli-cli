using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
// ReSharper disable once UnusedType.Global
public class RestartAnalysisActivityTest :
    StartAnalysisActivityTestBase<RestartAnalysisActivity, UnableToRestartAnalysisEvent>
{
    protected override RestartAnalysisActivity Activity => new(CacheManager.Object, IntervalParser.Object)
    {
        CacheDirectory = "example",
        RepositoryUrl = "http://git.example.com",
        RepositoryBranch = "main",
        HistoryInterval = "1m"
    };
}
