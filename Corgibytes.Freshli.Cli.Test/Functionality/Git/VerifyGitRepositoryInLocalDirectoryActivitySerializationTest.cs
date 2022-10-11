using System;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class VerifyGitRepositoryInLocalDirectoryActivitySerializationTest :
    SerializationTest<VerifyGitRepositoryInLocalDirectoryActivity>
{
    protected override VerifyGitRepositoryInLocalDirectoryActivity BuildIncoming() =>
        new() { AnalysisId = Guid.NewGuid() };

    protected override void AssertEqual(VerifyGitRepositoryInLocalDirectoryActivity incoming,
        VerifyGitRepositoryInLocalDirectoryActivity outgoing) => Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
}
