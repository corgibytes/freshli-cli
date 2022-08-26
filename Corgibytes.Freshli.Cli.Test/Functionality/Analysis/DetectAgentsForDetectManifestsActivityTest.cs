using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectAgentsForDetectManifestsActivityTest
{
    [Fact]
    public void Handle()
    {
        var agentPaths = new List<string>
        {
            "/usr/local/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-dotnet"
        };

        var agentsDetector = new Mock<IAgentsDetector>();
        agentsDetector.Setup(mock => mock.Detect()).Returns(agentPaths);

        var agentManager = new Mock<IAgentManager>();

        var javaAgentReader = new Mock<IAgentReader>();
        var dotnetAgentReader = new Mock<IAgentReader>();

        agentManager.Setup(mock => mock.GetReader("/usr/local/bin/freshli-agent-java")).Returns(javaAgentReader.Object);
        agentManager.Setup(mock => mock.GetReader("/usr/local/bin/freshli-agent-dotnet"))
            .Returns(dotnetAgentReader.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();

        var analysisLocation = new Mock<IAnalysisLocation>();

        var activity =
            new DetectAgentsForDetectManifestsActivity(agentsDetector.Object, agentManager.Object,
                analysisLocation.Object);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.AgentReader == javaAgentReader.Object)));

        eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.AgentReader == dotnetAgentReader.Object)));
    }
}
