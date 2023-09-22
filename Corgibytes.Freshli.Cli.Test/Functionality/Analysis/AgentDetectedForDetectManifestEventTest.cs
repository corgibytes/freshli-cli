using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AgentDetectedForDetectManifestEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        const string agentExecutablePath = "/path/to/agent";
        var analysisId = Guid.NewGuid();
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var appEvent = new AgentDetectedForDetectManifestEvent
        {
            AnalysisId = analysisId,
            Parent = parent.Object,
            AgentExecutablePath = agentExecutablePath
        };
        var cancellationToken = new CancellationToken(false);

        var activityEngine = new Mock<IApplicationActivityEngine>();

        await appEvent.Handle(activityEngine.Object, cancellationToken);

        activityEngine.Verify(mock =>
            mock.Dispatch(
                It.Is<DetectManifestsUsingAgentActivity>(activity =>
                    activity.AnalysisId == analysisId &&
                    activity.Parent == appEvent &&
                    activity.AgentExecutablePath == agentExecutablePath
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var cancellationToken = new CancellationToken(false);
        var appEvent = new AgentDetectedForDetectManifestEvent
        {
            AnalysisId = Guid.NewGuid(),
            Parent = parent.Object,
            AgentExecutablePath = "/path/to/agent"
        };

        var activityEngine = new Mock<IApplicationActivityEngine>();

        var exception = new InvalidOperationException();
        activityEngine.Setup(mock =>
            mock.Dispatch(
                It.IsAny<DetectManifestsUsingAgentActivity>(),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        ).Throws(exception);

        await appEvent.Handle(activityEngine.Object, cancellationToken);

        activityEngine.Verify(mock =>
            mock.Dispatch(
                It.Is<FireHistoryStopPointProcessingErrorActivity>(activity =>
                    activity.Parent == appEvent &&
                    activity.Error == exception
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
