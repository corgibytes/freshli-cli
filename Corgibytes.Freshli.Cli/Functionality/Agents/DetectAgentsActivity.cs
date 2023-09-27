using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class DetectAgentsActivity : IApplicationActivity
{
    private readonly IAgentsDetector _agentsDetector;

    public DetectAgentsActivity(IAgentsDetector agentsDetector) => _agentsDetector = agentsDetector;

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var agents = _agentsDetector.Detect();

        // Path.GetFileName returns string?, but ToDictionary needs string. We resolve this with the following, and
        // then tell the compiler to stop complaining that GetFileName(x) is never null; this is about the return type.
        // ReSharper disable once ConstantNullCoalescingCondition
        var agentsAndLocations = agents.ToDictionary(
            x => Path.GetFileName(x) ?? throw new ArgumentException("No file name for given path.")
        );

        await eventClient.Fire(new AgentsDetectedEvent { AgentsAndLocations = agentsAndLocations }, cancellationToken);
    }
}
