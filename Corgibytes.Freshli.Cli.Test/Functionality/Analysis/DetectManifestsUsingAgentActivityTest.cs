using System;
using System.Collections.Generic;
using System.IO;
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
            new List<string> { "/path/to/first/manifest", "/path/to/second/manifest" });

        var activity = new DetectManifestsUsingAgentActivity(analysisLocation.Object, agentReader.Object);

        var eventEngine = new Mock<IApplicationEventEngine>();

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisLocation == analysisLocation.Object &&
            appEvent.AgentReader == agentReader.Object &&
            appEvent.ManifestPath == "/path/to/first/manifest")));

        eventEngine.Verify(mock => mock.Fire(It.Is<ManifestDetectedEvent>(appEvent =>
            appEvent.AnalysisLocation == analysisLocation.Object &&
            appEvent.AgentReader == agentReader.Object &&
            appEvent.ManifestPath == "/path/to/second/manifest")));
    }
}
