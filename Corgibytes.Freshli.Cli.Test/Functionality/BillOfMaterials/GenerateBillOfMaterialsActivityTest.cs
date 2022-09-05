using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivityTest
{
    [Fact]
    public void Handle()
    {
        // Arrange
        var agentManager = new Mock<IAgentManager>();
        var javaAgentReader = new Mock<IAgentReader>();
        var eventEngine = new Mock<IApplicationEventEngine>();
        var analysisLocation = new Mock<IAnalysisLocation>();

        agentManager.Setup(mock => mock.GetReader("/usr/local/bin/freshli-agent-java")).Returns(javaAgentReader.Object);
        javaAgentReader.Setup(mock => mock.ProcessManifest("/path/to/manifest", It.IsAny<DateTime>())).Returns("/path/to/bill-of-materials");

        // Act
        var activity = new GenerateBillOfMaterialsActivity(
            agentManager.Object, analysisLocation.Object, "/path/to/manifest", "/usr/local/bin/freshli-agent-java"
        );
        activity.Handle(eventEngine.Object);

        // Assert
        eventEngine.Verify(mock =>
            mock.Fire(It.Is<BillOfMaterialsGeneratedEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.PathToBillOfMaterials == "/path/to/bill-of-materials")));
    }
}
