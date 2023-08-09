using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivityTest
{
    [Fact(Timeout = 500)]
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
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPointId).Returns(historyStopPointId);

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);

        var fileValidator = new Mock<IFileValidator>();
        fileValidator.Setup(mock => mock.IsValidFilePath("/path/to/bill-of-materials")).Returns(true);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IFileValidator))).Returns(fileValidator.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        // Act
        var analysisId = Guid.NewGuid();

        cacheManager.Setup(mock => mock.StoreBomInCache("/path/to/bill-of-materials", analysisId, asOfDateTime))
            .ReturnsAsync("/path/to/bom/in/cache");


        var cancellationToken = new CancellationToken(false);
        var activity = new GenerateBillOfMaterialsActivity(
            analysisId,
            agentExecutablePath,
            parent.Object,
            "/path/to/manifest");
        await activity.Handle(eventEngine.Object, cancellationToken);

        // Assert
        eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<BillOfMaterialsGeneratedEvent>(appEvent =>
                    appEvent.AgentExecutablePath == agentExecutablePath &&
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == parent.Object &&
                    appEvent.PathToBillOfMaterials == "/path/to/bom/in/cache"
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var eventEngine = new Mock<IApplicationEventEngine>();

        var exception = new InvalidOperationException("Simulated exception");

        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(exception);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);


        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var cancellationToken = new CancellationToken(false);
        var activity = new GenerateBillOfMaterialsActivity(
            Guid.NewGuid(), "/path/to/agent", parent.Object, "/path/to/manifest");
        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == activity.Parent &&
                    value.Exception == exception
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
