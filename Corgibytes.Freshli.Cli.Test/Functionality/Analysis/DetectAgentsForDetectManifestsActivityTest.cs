using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectAgentsForDetectManifestsActivityTest
{
    private readonly Mock<IAgentsDetector> _agentsDetector = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IHistoryStopData> _historyStopData = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();


    public DetectAgentsForDetectManifestsActivityTest()
    {
        _serviceProvider.Setup(mock => mock.GetService(typeof(IAgentsDetector))).Returns(_agentsDetector.Object);
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    [Fact]
    public void VerifyItDispatchesAgentDetectedForDetectManifestEvent()
    {
        var agentPaths = new List<string>
        {
            "/usr/local/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-dotnet"
        };

        _agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var analysisId = Guid.NewGuid();
        var activity =
            new DetectAgentsForDetectManifestsActivity(analysisId, _historyStopData.Object);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisId == analysisId &&
                appEvent.HistoryStopData == _historyStopData.Object &&
                appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-java")));

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisId == analysisId &&
                appEvent.HistoryStopData == _historyStopData.Object &&
                appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-dotnet")));
    }

    [Fact]
    public void VerifyItDispatchesNoAgentsDetectedFailureEvent()
    {
        var agentPaths = new List<string>();

        _agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var analysisId = Guid.NewGuid();
        var activity = new DetectAgentsForDetectManifestsActivity(analysisId, _historyStopData.Object);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock => mock.Fire(It.Is<NoAgentsDetectedFailureEvent>(
            failEvent => failEvent.ErrorMessage == "Could not locate any agents"
        )));
    }
}
