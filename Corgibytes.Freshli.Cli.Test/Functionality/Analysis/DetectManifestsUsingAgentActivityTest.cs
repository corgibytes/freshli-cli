using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectManifestsUsingAgentActivityTest
{
    [Fact]
    public void HandleWritesManifestPathsToCache()
    {
        var localPath = "/path/to/repository";
        var agentReader = new Mock<IAgentReader>();
        var manifestPaths = new List<string>
        {
            "/path/to/first/manifest",
            "/path/to/second/manifest"
        };
        agentReader.Setup(mock => mock.DetectManifests(localPath)).Returns(manifestPaths);

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath)).Returns(agentReader.Object);

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { LocalPath = localPath };

        var historyStopPointId = 29;
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).Returns(historyStopPoint);
        cacheDb.Setup(mock => mock.RetrieveCachedManifests(historyStopPointId, agentExecutablePath))
            .Returns(new List<string>());

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();
        var activity =
            new DetectManifestsUsingAgentActivity(analysisId, historyStopPointId, agentExecutablePath);

        activity.Handle(eventEngine.Object);

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

        cacheDb.Verify(mock => mock.StoreCachedManifests(historyStopPointId, agentExecutablePath, manifestPaths));
    }

    [Fact]
    public void HandleReadsManifestPathsFromCache()
    {
        var localPath = "/path/to/repository";
        var agentReader = new Mock<IAgentReader>();
        var manifestPaths = new List<string>
        {
            "/path/to/first/manifest",
            "/path/to/second/manifest"
        };
        agentReader.Setup(mock => mock.DetectManifests(localPath)).Returns(manifestPaths);

        const string agentExecutablePath = "/path/to/agent";

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { LocalPath = localPath };

        var historyStopPointId = 29;
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).Returns(historyStopPoint);
        cacheDb.Setup(mock => mock.RetrieveCachedManifests(historyStopPointId, agentExecutablePath))
            .Returns(manifestPaths);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisId = Guid.NewGuid();
        var activity =
            new DetectManifestsUsingAgentActivity(analysisId, historyStopPointId, agentExecutablePath);

        activity.Handle(eventEngine.Object);

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
}
