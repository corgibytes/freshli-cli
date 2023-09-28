using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivityTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        const string repositoryPath = "/path/to/repository";
        const string manifestPath = "path/to/manifest";
        var fullManifestPath = Path.Combine(repositoryPath, manifestPath);

        var asOfDateTime = DateTimeOffset.Now;
        var javaAgentReader = new Mock<IAgentReader>();
        javaAgentReader.Setup(mock => mock.ProcessManifest(fullManifestPath, asOfDateTime))
            .ReturnsAsync("/path/to/bill-of-materials");

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath, CancellationToken.None)).Returns(javaAgentReader.Object);

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var analysisId = Guid.NewGuid();
        var historyStopPoint = new CachedHistoryStopPoint
        {
            Id = 29,
            LocalPath = repositoryPath,
            AsOfDateTime = asOfDateTime,
            CachedAnalysis = new CachedAnalysis { Id = analysisId }
        };
        var manifest = new CachedManifest
        {
            Id = 12,
            ManifestFilePath = manifestPath,
            HistoryStopPoint = historyStopPoint
        };

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPoint).Returns(historyStopPoint);
        parent.Setup(mock => mock.Manifest).Returns(manifest);

        cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(cacheDb.Object);

        var fileValidator = new Mock<IFileValidator>();
        fileValidator.Setup(mock => mock.IsValidFilePath("/path/to/bill-of-materials")).Returns(true);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IFileValidator))).Returns(fileValidator.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        cacheManager.Setup(mock => mock.StoreBomInCache("/path/to/bill-of-materials", analysisId, asOfDateTime, manifestPath))
            .ReturnsAsync("/path/to/bom/in/cache");

        var cancellationToken = new CancellationToken(false);
        var activity = new GenerateBillOfMaterialsActivity
        {
            AgentExecutablePath = agentExecutablePath,
            Parent = parent.Object,
        };
        await activity.Handle(eventEngine.Object, cancellationToken);

        // Assert
        eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<BillOfMaterialsGeneratedEvent>(appEvent =>
                    appEvent.AgentExecutablePath == agentExecutablePath &&
                    appEvent.Parent == activity &&
                    appEvent.PathToBillOfMaterials == "/path/to/bom/in/cache"
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
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
        parent.Setup(mock => mock.HistoryStopPoint).Returns(new CachedHistoryStopPoint { Id = 12 });
        parent.Setup(mock => mock.Manifest).Returns(new CachedManifest { Id = 24 });
        var cancellationToken = new CancellationToken(false);
        var activity = new GenerateBillOfMaterialsActivity
        {
            AgentExecutablePath = "/path/to/agent",
            Parent = parent.Object
        };
        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == activity &&
                    value.Exception == exception
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
