using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectManifestsUsingAgentActivityTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        const string localPath = "/path/to/repository";
        var agentReader = new Mock<IAgentReader>();
        agentReader.Setup(mock => mock.DetectManifests(localPath)).Returns(
            new List<string>
            {
                "/path/to/first/manifest",
                "/path/to/second/manifest"
            }.ToAsyncEnumerable());

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        var cancellationToken = new CancellationToken(false);
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath, cancellationToken)).Returns(agentReader.Object);

        var historyStopPoint = new CachedHistoryStopPoint { Id = 29, LocalPath = localPath };

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPoint).Returns(historyStopPoint);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();
        var activity = new DetectManifestsUsingAgentActivity
        {
            AnalysisId = analysisId,
            Parent = parent.Object,
            AgentExecutablePath = agentExecutablePath
        };

        var firstCachedManifest = new CachedManifest { ManifestFilePath = "/path/to/first/manifest" };
        var secondCachedManifest = new CachedManifest { ManifestFilePath = "/path/to/second/manifest" };

        var cacheManager = new Mock<ICacheManager>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        var cacheDb = new Mock<ICacheDb>();
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        cacheDb.Setup(mock => mock.AddManifest(historyStopPoint, "/path/to/first/manifest"))
            .ReturnsAsync(firstCachedManifest);
        cacheDb.Setup(mock => mock.AddManifest(historyStopPoint, "/path/to/second/manifest"))
            .ReturnsAsync(secondCachedManifest);

        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<ManifestDetectedEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == activity &&
                    appEvent.AgentExecutablePath == agentExecutablePath &&
                    appEvent.Manifest == firstCachedManifest
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<ManifestDetectedEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == activity &&
                    appEvent.AgentExecutablePath == agentExecutablePath &&
                    appEvent.Manifest == secondCachedManifest
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

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPoint).Returns(new CachedHistoryStopPoint { Id = 12 });

        var cancellationToken = new CancellationToken(false);
        var activity = new DetectManifestsUsingAgentActivity
        {
            AnalysisId = Guid.NewGuid(),
            Parent = parent.Object,
            AgentExecutablePath = "/path/to/agent"
        };

        var agentManager = new Mock<IAgentManager>();
        var exception = new InvalidOperationException("Simulated exception");
        agentManager.Setup(mock => mock.GetReader(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(exception);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        var cacheManager = new Mock<ICacheManager>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        var cacheDb = new Mock<ICacheDb>();
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(appEvent =>
                    appEvent.Parent == activity &&
                    appEvent.Exception == exception
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyFiresNoManifestsDetectedEvent()
    {
        const string localPath = "/path/to/repository";
        var agentReader = new Mock<IAgentReader>();
        agentReader.Setup(mock => mock.DetectManifests(localPath)).Returns(
            new List<string>().ToAsyncEnumerable());

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath, CancellationToken.None)).Returns(agentReader.Object);

        var historyStopPoint = new CachedHistoryStopPoint { Id = 29, LocalPath = localPath };

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPoint).Returns(historyStopPoint);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        var cacheManager = new Mock<ICacheManager>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        var cacheDb = new Mock<ICacheDb>();
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();

        var cancellationToken = new CancellationToken(false);
        var activity = new DetectManifestsUsingAgentActivity
        {
            AnalysisId = analysisId,
            Parent = parent.Object,
            AgentExecutablePath = agentExecutablePath
        };

        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<NoManifestsDetectedEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == activity
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
