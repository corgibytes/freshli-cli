using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class StartAnalysisActivityTest : StartAnalysisActivityTestBase<StartAnalysisActivity, CacheWasNotPreparedEvent>
{
    protected override StartAnalysisActivity Activity => new(_cacheManager.Object, _intervalParser.Object)
    {
        CacheDirectory = "example",
        RepositoryUrl = "http://git.example.com",
        RepositoryBranch = "main",
        HistoryInterval = "1m"
    };

    protected override Func<CacheWasNotPreparedEvent, bool> EventValidator
    {
        get
        {
            return (value) =>
                value.CacheDirectory == "example" &&
                value.RepositoryUrl == "http://git.example.com" &&
                value.RepositoryBranch == "main" &&
                value.HistoryInterval == "1m";
        }
    }
}
