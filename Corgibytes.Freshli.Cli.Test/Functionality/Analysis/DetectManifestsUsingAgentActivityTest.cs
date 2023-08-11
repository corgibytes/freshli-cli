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

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { LocalPath = localPath };

        const int historyStopPointId = 29;

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPointId).Returns(historyStopPointId);

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();
        var activity =
            new DetectManifestsUsingAgentActivity(analysisId, parent.Object, agentExecutablePath);

        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<ManifestDetectedEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == parent.Object &&
                    appEvent.AgentExecutablePath == agentExecutablePath &&
                    appEvent.ManifestPath == "/path/to/first/manifest"
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<ManifestDetectedEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == parent.Object &&
                    appEvent.AgentExecutablePath == agentExecutablePath &&
                    appEvent.ManifestPath == "/path/to/second/manifest"
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
        var cancellationToken = new CancellationToken(false);
        var activity = new DetectManifestsUsingAgentActivity(Guid.NewGuid(), parent.Object, "/path/to/agent");

        var agentManager = new Mock<IAgentManager>();
        var exception = new InvalidOperationException("Simulated exception");
        agentManager.Setup(mock => mock.GetReader(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(exception);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(appEvent =>
                    appEvent.Parent == activity.Parent &&
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

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { LocalPath = localPath };

        const int historyStopPointId = 29;
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPointId).Returns(historyStopPointId);

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();

        var cancellationToken = new CancellationToken(false);
        var activity =
            new DetectManifestsUsingAgentActivity(analysisId, parent.Object, agentExecutablePath);

        await activity.Handle(eventEngine.Object, cancellationToken);

        eventEngine.Verify(
            mock => mock.Fire(
                It.Is<NoManifestsDetectedEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == parent.Object
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
