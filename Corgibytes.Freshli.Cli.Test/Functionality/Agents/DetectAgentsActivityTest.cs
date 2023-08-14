using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Agents;

[UnitTest]
public class DetectAgentsActivityTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyItFiresAgentsDetectedEvent()
    {
        var eventClient = new Mock<IApplicationEventEngine>();
        var agentsDetector = new Mock<IAgentsDetector>();
        var activity = new DetectAgentsActivity(agentsDetector.Object);
        var cancellationToken = new CancellationToken(false);

        const string pathToAgent = "/agent/smith";

        agentsDetector.Setup(mock => mock.Detect()).Returns(new List<string> { pathToAgent });

        await activity.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(mock =>
            mock.Fire(
                It.Is<AgentsDetectedEvent>(agentsEvent => agentsEvent.AgentsAndLocations!.ContainsValue(pathToAgent)),
                cancellationToken,
                ApplicationTaskMode.Tracked)
        );
    }
}
