using System;
using System.Collections.Generic;
using System.Linq;
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
    [Fact]
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
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath)).Returns(agentReader.Object);

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { LocalPath = localPath };

        const int historyStopPointId = 29;
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();
        var activity =
            new DetectManifestsUsingAgentActivity(analysisId, historyStopPointId, agentExecutablePath);

        await activity.Handle(eventEngine.Object);

        eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisId == analysisId &&
            appEvent.HistoryStopPointId == historyStopPointId &&
            appEvent.AgentExecutablePath == agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/first/manifest")));

        eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisId == analysisId &&
            appEvent.HistoryStopPointId == historyStopPointId &&
            appEvent.AgentExecutablePath == agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/second/manifest")));
    }

    [Fact]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var eventEngine = new Mock<IApplicationEventEngine>();

        var exception = new InvalidOperationException();
        eventEngine.Setup(mock => mock.ServiceProvider).Throws(exception);

        var activity = new DetectManifestsUsingAgentActivity(Guid.NewGuid(), 29, "/path/to/agent");

        await activity.Handle(eventEngine.Object);

        eventEngine.Verify(mock => mock.Fire(It.Is<HistoryStopPointProcessingFailedEvent>(appEvent =>
            appEvent.HistoryStopPointId == activity.HistoryStopPointId &&
            appEvent.Error == exception
        )));
    }
}
