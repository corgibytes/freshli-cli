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
    private readonly string _localPath;
    private readonly Mock<IAgentReader> _agentReader;
    private readonly List<string> _manifestPaths;
    private readonly Mock<IAgentManager> _agentManager;
    private readonly Mock<ICacheDb> _cacheDb;
    private readonly int _historyStopPointId;
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly Mock<IApplicationEventEngine> _eventEngine;
    private readonly Guid _analysisId;
    private readonly string _agentExecutablePath;
    private readonly DetectManifestsUsingAgentActivity _activity;

    public DetectManifestsUsingAgentActivityTest()
    {
        _localPath = "/path/to/repository";
        _agentReader = new Mock<IAgentReader>();
        _manifestPaths = new List<string>
        {
            "/path/to/first/manifest",
            "/path/to/second/manifest"
        };
        _agentManager = new Mock<IAgentManager>();
        var cacheManager = new Mock<ICacheManager>();
        _cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { LocalPath = _localPath };
        _historyStopPointId = 29;
        _serviceProvider = new Mock<IServiceProvider>();
        _eventEngine = new Mock<IApplicationEventEngine>();
        _analysisId = Guid.NewGuid();
        _agentExecutablePath = "/path/to/agent";
        _activity = new DetectManifestsUsingAgentActivity(_analysisId, _historyStopPointId, _agentExecutablePath);

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);
        _cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(_historyStopPointId)).Returns(historyStopPoint);
        _cacheDb.Setup(mock => mock.RetrieveCachedManifests(_historyStopPointId, _agentExecutablePath))
            .Returns(new List<string>());

        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    [Fact]
    public void HandleWritesManifestPathsToCache()
    {
        _serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(_agentManager.Object);
        _agentReader.Setup(mock => mock.DetectManifests(_localPath)).Returns(_manifestPaths);
        _agentManager.Setup(mock => mock.GetReader(_agentExecutablePath)).Returns(_agentReader.Object);

        _activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisId == _analysisId &&
            appEvent.HistoryStopPointId == _historyStopPointId &&
            appEvent.AgentExecutablePath == _agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/first/manifest")));

        _eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisId == _analysisId &&
            appEvent.HistoryStopPointId == _historyStopPointId &&
            appEvent.AgentExecutablePath == _agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/second/manifest")));

        _cacheDb.Verify(mock => mock.StoreCachedManifests(_historyStopPointId, _agentExecutablePath, _manifestPaths));
    }

    [Fact]
    public void HandleReadsManifestPathsFromCache()
    {
        _cacheDb.Setup(mock => mock.RetrieveCachedManifests(_historyStopPointId, _agentExecutablePath))
            .Returns(_manifestPaths);

        _activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisId == _analysisId &&
            appEvent.HistoryStopPointId == _historyStopPointId &&
            appEvent.AgentExecutablePath == _agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/first/manifest")));

        _eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisId == _analysisId &&
            appEvent.HistoryStopPointId == _historyStopPointId &&
            appEvent.AgentExecutablePath == _agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/second/manifest")));
    }
}
