using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
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
        var billOfMaterialsPath = "/path/to/bom";
        var analysisLocation = new AnalysisLocation("/cache/directory", "2dbc2fd2358e1ea1b7a6bc08ea647b9a337ac92d",
            "da39a3ee5e6b4b0d3255bfef95601890afd80709");

        var javaAgentReader = new Mock<IAgentReader>();

        javaAgentReader.Setup(mock => mock.ProcessManifest(manifestPath, It.IsAny<DateTime>())).Returns(billOfMaterialsPath);

        var engine = new Mock<IApplicationActivityEngine>();

        var manifestEvent = new ManifestDetectedEvent(analysisLocation, javaAgentReader.Object, manifestPath);
        manifestEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<GenerateBillOfMaterialsActivity>(value =>
            value.AnalysisLocation == analysisLocation &&
            value.ManifestPath == manifestPath &&
            value.AgentReader == javaAgentReader.Object
        )));
    }
}
