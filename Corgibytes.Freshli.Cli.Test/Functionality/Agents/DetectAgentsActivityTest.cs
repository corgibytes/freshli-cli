using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Agents;

[UnitTest]
public class DetectAgentsActivityTest
{
    [Fact]
    public void VerifyItFiresAgentsDetectedEvent()
    {
        var eventClient = new Mock<IApplicationEventEngine>();
        var agentsDetector = new Mock<IAgentsDetector>();
        var activity = new DetectAgentsActivity(agentsDetector.Object);

        const string pathToAgent = "/agent/smith";

        agentsDetector.Setup(mock => mock.Detect()).Returns(new List<string> { pathToAgent });

        activity.Handle(eventClient.Object);

        eventClient.Verify(mock =>
            mock.Fire(It.Is<AgentsDetectedEvent>(agentsEvent =>
                agentsEvent.AgentsAndLocations!.ContainsValue(pathToAgent)
            ))
        );
    }
}
