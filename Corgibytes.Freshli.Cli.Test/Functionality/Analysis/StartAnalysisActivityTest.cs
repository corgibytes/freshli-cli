using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
// ReSharper disable once UnusedType.Global
public class StartAnalysisActivityTest : StartAnalysisActivityTestBase<StartAnalysisActivity, CacheDoesNotExistEvent>
{
    protected override StartAnalysisActivity Activity => new()
    {
        RepositoryUrl = "http://git.example.com",
        RepositoryBranch = "main",
        HistoryInterval = "1m"
    };

    protected override Func<CacheDoesNotExistEvent, bool> EventValidator =>
        value =>
            value is { RepositoryUrl: "http://git.example.com", RepositoryBranch: "main", HistoryInterval: "1m" };
}
