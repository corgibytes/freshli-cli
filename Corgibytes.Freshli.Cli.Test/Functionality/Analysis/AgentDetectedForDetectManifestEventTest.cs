using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AgentDetectedForDetectManifestEventTest
{
    [Fact]
    public void Handle()
    {
        var analysisLocation = new Mock<IAnalysisLocation>();
        var agentReader = new Mock<IAgentReader>();

        var appEvent = new AgentDetectedForDetectManifestEvent(analysisLocation.Object, agentReader.Object);

        var activityEngine = new Mock<IApplicationActivityEngine>();

        appEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock =>
            mock.Dispatch(It.Is<DetectManifestsUsingAgentActivity>(activity =>
                activity.AnalysisLocation == analysisLocation.Object &&
                activity.AgentReader == agentReader.Object)));
    }
}
