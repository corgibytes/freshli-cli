using System;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class CloneGitRepositoryActivitySerializationTest : SerializationTest<CloneGitRepositoryActivity>
{
    protected override CloneGitRepositoryActivity BuildIncoming() =>
        new(
            Guid.NewGuid()
        );

    protected override void AssertEqual(CloneGitRepositoryActivity incoming, CloneGitRepositoryActivity outgoing)
    {
        Assert.Equal(incoming.CachedAnalysisId, outgoing.CachedAnalysisId);
    }
}

