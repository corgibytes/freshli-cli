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
        var javaAgentReader = new Mock<IAgentReader>();
        var eventEngine = new Mock<IApplicationEventEngine>();
        var analysisLocation = new Mock<IAnalysisLocation>();

        javaAgentReader.Setup(mock => mock.ProcessManifest("/path/to/manifest", It.IsAny<DateTime>()))
            .Returns("/path/to/bill-of-materials");

        // Act
        var activity = new GenerateBillOfMaterialsActivity(javaAgentReader.Object, analysisLocation.Object, "/path/to/manifest");
        activity.Handle(eventEngine.Object);

        // Assert
        eventEngine.Verify(mock =>
            mock.Fire(It.Is<BillOfMaterialsGeneratedEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.PathToBillOfMaterials == "/path/to/bill-of-materials")));
    }
}
