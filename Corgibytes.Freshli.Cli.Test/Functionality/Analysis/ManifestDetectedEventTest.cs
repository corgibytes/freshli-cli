using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class ManifestDetectedEventTest
{
    [Fact]
    public void CorrectlyDispatchesGenerateBillOfMaterialsActivity()
    {
        var manifestPath = "/path/to/manifest";
        var engine = new Mock<IApplicationActivityEngine>();

        const string agentExecutablePath = "/path/to/agent";
        var analysisId = Guid.NewGuid();
        var historyStopPointId = 29;
        var manifestEvent =
            new ManifestDetectedEvent(analysisId, historyStopPointId, agentExecutablePath, manifestPath);
        manifestEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<GenerateBillOfMaterialsActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.ManifestPath == manifestPath &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }
}
