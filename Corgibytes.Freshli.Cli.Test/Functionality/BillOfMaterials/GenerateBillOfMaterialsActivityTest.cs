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
        javaAgentReader.Setup(mock => mock.ProcessManifest("/path/to/manifest", It.IsAny<DateTime>()))
            .Returns("/path/to/bill-of-materials");

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath)).Returns(javaAgentReader.Object);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        // Act
        var analysisLocation = new Mock<IAnalysisLocation>();
        var activity = new GenerateBillOfMaterialsActivity(agentExecutablePath, analysisLocation.Object, "/path/to/manifest");
        activity.Handle(eventEngine.Object);

        // Assert
        eventEngine.Verify(mock =>
            mock.Fire(It.Is<BillOfMaterialsGeneratedEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.PathToBillOfMaterials == "/path/to/bill-of-materials")));
    }
}
