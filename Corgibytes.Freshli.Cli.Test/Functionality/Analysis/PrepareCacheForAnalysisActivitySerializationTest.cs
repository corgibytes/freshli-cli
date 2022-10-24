using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class PrepareCacheForAnalysisActivitySerializationTest : SerializationTest<PrepareCacheForAnalysisActivity>
{
    protected override PrepareCacheForAnalysisActivity BuildIncoming() => new(
        "https://actual-repository-url.com",
        "trunk",
        "1m",
        CommitHistory.Full,
        RevisionHistoryMode.OnlyLatestRevision
    );

    protected override void AssertEqual(PrepareCacheForAnalysisActivity incoming,
        PrepareCacheForAnalysisActivity outgoing)
    {
        Assert.Equal(incoming.RepositoryUrl, outgoing.RepositoryUrl);
        Assert.Equal(incoming.RepositoryBranch, outgoing.RepositoryBranch);
        Assert.Equal(incoming.HistoryInterval, outgoing.HistoryInterval);
        Assert.Equal(incoming.UseCommitHistory, outgoing.UseCommitHistory);
        Assert.Equal(incoming.RevisionHistoryMode, outgoing.RevisionHistoryMode);
    }
}
