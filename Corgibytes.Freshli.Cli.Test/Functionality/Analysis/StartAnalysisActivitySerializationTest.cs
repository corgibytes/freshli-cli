using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class StartAnalysisActivitySerializationTest : SerializationTest<StartAnalysisActivity>
{
    protected override StartAnalysisActivity BuildIncoming() =>
        new()
        {
            HistoryInterval = "1m",
            RepositoryBranch = "trunk",
            RepositoryUrl = "https://github.com/corgibytes/freshli-cli",
            RevisionHistoryMode = RevisionHistoryMode.OnlyLatestRevision,
            UseCommitHistory = CommitHistory.AtInterval
        };

    protected override void AssertEqual(StartAnalysisActivity incoming, StartAnalysisActivity outgoing)
    {
        Assert.Equal(incoming.HistoryInterval, outgoing.HistoryInterval);
        Assert.Equal(incoming.RepositoryBranch, outgoing.RepositoryBranch);
        Assert.Equal(incoming.RepositoryUrl, outgoing.RepositoryUrl);
        Assert.Equal(incoming.RevisionHistoryMode, outgoing.RevisionHistoryMode);
        Assert.Equal(incoming.UseCommitHistory, outgoing.UseCommitHistory);
    }
}
