using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class RestartAnalysisActivityTest : StartAnalysisActivityTestBase<RestartAnalysisActivity, UnableToRestartAnalysisEvent>
{
    protected override RestartAnalysisActivity Activity => new(_cacheManager.Object, _intervalParser.Object)
    {
        CacheDirectory = "example",
        RepositoryUrl = "http://git.example.com",
        RepositoryBranch = "main",
        HistoryInterval = "1m"
    };
}
