using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectAgentsForDetectManifestsActivityTest
{
    private const int HistoryStopPointId = 29;
    private readonly Mock<IAgentsDetector> _agentsDetector = new();
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<CachedHistoryStopPoint> _historyStopPoint = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public DetectAgentsForDetectManifestsActivityTest()
    {
        _cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(HistoryStopPointId)).ReturnsAsync(_historyStopPoint.Object);
        _cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);

        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IAgentsDetector))).Returns(_agentsDetector.Object);
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    [Fact]
    public async ValueTask VerifyItDispatchesAgentDetectedForDetectManifestEvent()
    {
        var agentPaths = new List<string>
        {
            "/usr/local/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-dotnet"
        };

        _agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var activity =
            new DetectAgentsForDetectManifestsActivity(analysisId, historyStopPointId);

        await activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisId == analysisId &&
                appEvent.HistoryStopPointId == historyStopPointId &&
                appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-java")));

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisId == analysisId &&
                appEvent.HistoryStopPointId == historyStopPointId &&
                appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-dotnet")));
    }

    [Fact]
    public async ValueTask VerifyItDispatchesNoAgentsDetectedFailureEvent()
    {
        var agentPaths = new List<string>();

        _agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var analysisId = Guid.NewGuid();
        var activity = new DetectAgentsForDetectManifestsActivity(analysisId, HistoryStopPointId);

        await activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock => mock.Fire(It.Is<NoAgentsDetectedFailureEvent>(
            failEvent => failEvent.ErrorMessage == "Could not locate any agents"
        )));
    }
}
