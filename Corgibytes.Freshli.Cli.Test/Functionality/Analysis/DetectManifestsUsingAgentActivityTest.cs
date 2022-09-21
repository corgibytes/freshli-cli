using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetectManifestsUsingAgentActivityTest
{
    [Fact]
    public void Handle()
    {
        var analysisLocation = new Mock<IAnalysisLocation>();
        analysisLocation.SetupGet(mock => mock.Path).Returns("/path/to/repository");

        var agentReader = new Mock<IAgentReader>();
        agentReader.Setup(mock => mock.DetectManifests("/path/to/repository")).Returns(
            new List<string>
            {
                "/path/to/first/manifest",
                "/path/to/second/manifest"
            });

        const string agentExecutablePath = "/path/to/agent";
        var agentManager = new Mock<IAgentManager>();
        agentManager.Setup(mock => mock.GetReader(agentExecutablePath)).Returns(agentReader.Object);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var activity = new DetectManifestsUsingAgentActivity(analysisLocation.Object, agentExecutablePath);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisLocation == analysisLocation.Object &&
            appEvent.AgentExecutablePath == agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/first/manifest")));

        eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisLocation == analysisLocation.Object &&
            appEvent.AgentExecutablePath == agentExecutablePath &&
            appEvent.ManifestPath == "/path/to/second/manifest")));
    }
}
