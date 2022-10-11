using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class DetectManifestsUsingAgentActivitySerializationTest : SerializationTest<DetectManifestsUsingAgentActivity>
{
    protected override DetectManifestsUsingAgentActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            29,
            "/agent/jones"
        );

    protected override void AssertEqual(DetectManifestsUsingAgentActivity incoming,
        DetectManifestsUsingAgentActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
    }
}
