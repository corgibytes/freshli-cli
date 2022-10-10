using System;
using Corgibytes.Freshli.Cli.Functionality;
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
        var configuration = new Mock<IConfiguration>();
        var manifestPath = "/path/to/manifest";
        var historyStopData = new HistoryStopData(configuration.Object, "2dbc2fd2358e1ea1b7a6bc08ea647b9a337ac92d",
            "da39a3ee5e6b4b0d3255bfef95601890afd80709");

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
