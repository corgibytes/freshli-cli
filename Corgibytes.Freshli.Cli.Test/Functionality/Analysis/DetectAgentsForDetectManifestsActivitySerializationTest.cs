using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class
    DetectAgentsForDetectManifestsActivitySerializationTest : SerializationTest<DetectAgentsForDetectManifestsActivity>
{
    protected override DetectAgentsForDetectManifestsActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            this.BuildHistoryStopData()
        );

    protected override void AssertEqual(DetectAgentsForDetectManifestsActivity incoming,
        DetectAgentsForDetectManifestsActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
    }
}
