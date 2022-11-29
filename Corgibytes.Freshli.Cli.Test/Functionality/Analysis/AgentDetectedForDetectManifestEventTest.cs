using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AgentDetectedForDetectManifestEventTest
{
    [Fact]
    public async Task Handle()
    {
        const string agentExecutablePath = "/path/to/agent";
        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var appEvent =
            new AgentDetectedForDetectManifestEvent(analysisId, historyStopPointId, agentExecutablePath);

        var activityEngine = new Mock<IApplicationActivityEngine>();

        await appEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock =>
            mock.Dispatch(It.Is<DetectManifestsUsingAgentActivity>(activity =>
                activity.AnalysisId == analysisId &&
                activity.HistoryStopPointId == historyStopPointId &&
                activity.AgentExecutablePath == agentExecutablePath)));
    }
}
