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

        var eventEngine = new Mock<IApplicationEventEngine>();

        var repositoryId = Guid.NewGuid().ToString();
        var commitId = Guid.NewGuid().ToString();

        var activity = new DetectAgentsForDetectManifestsActivity(agentsDetector.Object, repositoryId, commitId);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(
            mock => mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(
                appEvent =>
                    appEvent.RepositoryId == repositoryId &&
                    appEvent.CommitId == commitId &&
                    appEvent.AgentPath == "/usr/local/bin/freshli-agent-java")));

        eventEngine.Verify(
            mock => mock.Fire(It.Is<AgentDetectedForDetectManifestEvent>(
                appEvent =>
                    appEvent.RepositoryId == repositoryId &&
                    appEvent.CommitId == commitId &&
                    appEvent.AgentPath == "/usr/local/bin/freshli-agent-dotnet")));

    }
}
