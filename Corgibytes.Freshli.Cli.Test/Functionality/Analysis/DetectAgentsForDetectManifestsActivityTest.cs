using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
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

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentsDetector))).Returns(agentsDetector.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var analysisLocation = new Mock<IAnalysisLocation>();
        var activity =
            new DetectAgentsForDetectManifestsActivity(analysisLocation.Object);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-java")));

        eventEngine.Verify(mock =>
            mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(appEvent =>
                appEvent.AnalysisLocation == analysisLocation.Object &&
                appEvent.AgentExecutablePath == "/usr/local/bin/freshli-agent-dotnet")));
    }
}
