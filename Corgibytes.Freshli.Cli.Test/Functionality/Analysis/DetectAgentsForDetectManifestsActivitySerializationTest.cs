using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class DetectAgentsForDetectManifestsActivitySerializationTest : SerializationTest<DetectAgentsForDetectManifestsActivity>
{
    protected override DetectAgentsForDetectManifestsActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            29
        );

    protected override void AssertEqual(DetectAgentsForDetectManifestsActivity incoming,
        DetectAgentsForDetectManifestsActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
    }
}
