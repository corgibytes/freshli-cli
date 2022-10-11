using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.Functionality;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Cache;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class PrepareCacheActivitySerializationTest : SerializationTest<PrepareCacheActivity>
{
    protected override PrepareCacheActivity BuildIncoming() => new(
        "https://actual-repository-url.com",
        "trunk",
        "1m",
        CommitHistory.Full,
        RevisionHistoryMode.OnlyLatestRevision
    );

    protected override void AssertEqual(PrepareCacheActivity incoming, PrepareCacheActivity outgoing)
    {
        Assert.Equal(incoming.RepositoryUrl, outgoing.RepositoryUrl);
        Assert.Equal(incoming.RepositoryBranch, outgoing.RepositoryBranch);
        Assert.Equal(incoming.HistoryInterval, outgoing.HistoryInterval);
        Assert.Equal(incoming.UseCommitHistory, outgoing.UseCommitHistory);
        Assert.Equal(incoming.RevisionHistoryMode, outgoing.RevisionHistoryMode);
    }
}
