using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AgentDetectedForDetectManifestEventTest
{
    [Fact]
    public void Handle()
    {
        const string agentExecutablePath = "/path/to/agent";
        var analysisLocation = new Mock<IAnalysisLocation>();
        var appEvent = new AgentDetectedForDetectManifestEvent(analysisLocation.Object, agentExecutablePath);

        var activityEngine = new Mock<IApplicationActivityEngine>();

        appEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock =>
            mock.Dispatch(It.Is<DetectManifestsUsingAgentActivity>(activity =>
                activity.AnalysisLocation == analysisLocation.Object &&
                activity.AgentExecutablePath == agentExecutablePath)));
    }
}
