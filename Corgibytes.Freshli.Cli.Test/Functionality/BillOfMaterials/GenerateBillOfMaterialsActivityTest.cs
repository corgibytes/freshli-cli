using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivityTest
{
    [Fact]
    public async Task Handle()
    {
        // Arrange
        var asOfDateTime = DateTimeOffset.Now;
        var javaAgentReader = new Mock<IAgentReader>();
        javaAgentReader.Setup(mock => mock.ProcessManifest("/path/to/manifest", asOfDateTime))
            .ReturnsAsync("/path/to/bill-of-materials");

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath, CancellationToken.None)).Returns(javaAgentReader.Object);

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint
        {
            LocalPath = "/path/to/repository",
            AsOfDateTime = asOfDateTime
        };

        const int historyStopPointId = 29;

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        // Act
        var analysisId = Guid.NewGuid();

        cacheManager.Setup(mock => mock.StoreBomInCache("/path/to/bill-of-materials", analysisId, asOfDateTime))
            .ReturnsAsync("/path/to/bom/in/cache");

        var activity =
            new GenerateBillOfMaterialsActivity(analysisId, agentExecutablePath, historyStopPointId,
                "/path/to/manifest");
        await activity.Handle(eventEngine.Object);

        // Assert
        eventEngine.Verify(mock =>
            mock.Fire(It.Is<BillOfMaterialsGeneratedEvent>(appEvent =>
                appEvent.AgentExecutablePath == agentExecutablePath &&
                appEvent.AnalysisId == analysisId &&
                appEvent.HistoryStopPointId == historyStopPointId &&
                appEvent.PathToBillOfMaterials == "/path/to/bom/in/cache")));
    }
}
