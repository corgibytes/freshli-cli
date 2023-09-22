using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectAgentsForDetectManifestsActivityTest
{
    private readonly Mock<IAgentsDetector> _agentsDetector = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly CachedHistoryStopPoint _historyStopPoint = new() { Id = 29 };
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly CancellationToken _cancellationToken = new(false);
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();

    public DetectAgentsForDetectManifestsActivityTest()
    {
        _parent.Setup(mock => mock.HistoryStopPoint).Returns(_historyStopPoint);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IAgentsDetector))).Returns(_agentsDetector.Object);
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyItDispatchesAgentDetectedForDetectManifestEvent()
    {
        var agentPaths = new List<string>
        {
            "/usr/local/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-dotnet"
        };

        _agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var analysisId = Guid.NewGuid();
        var activity = new DetectAgentsForDetectManifestsActivity
        {
            AnalysisId = analysisId,
            Parent = _parent.Object
        };

        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == activity &&
                    appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-java"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                    appEvent.AnalysisId == analysisId &&
                    appEvent.Parent == activity &&
                    appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-dotnet"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyItDispatchesNoAgentsDetectedFailureEvent()
    {
        var agentPaths = new List<string>();

        _agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var analysisId = Guid.NewGuid();
        var activity = new DetectAgentsForDetectManifestsActivity
        {
            AnalysisId = analysisId,
            Parent = _parent.Object
        };

        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<NoAgentsDetectedFailureEvent>(
                    failEvent => failEvent.ErrorMessage == "Could not locate any agents"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var activity = new DetectAgentsForDetectManifestsActivity
        {
            AnalysisId = Guid.NewGuid(),
            Parent = _parent.Object
        };

        var exception = new InvalidOperationException("Simulated exception");
        _agentsDetector.Setup(mock => mock.Detect()).Throws(exception);

        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == activity &&
                    value.Exception == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
