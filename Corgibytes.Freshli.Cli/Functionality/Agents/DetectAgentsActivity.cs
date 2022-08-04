using System;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class DetectAgentsActivity : IApplicationActivity
{
    public DetectAgentsActivity(IAgentsDetector agentsDetector) => AgentsDetector = agentsDetector;

    private IAgentsDetector AgentsDetector { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agents = AgentsDetector.Detect();

        // Path.GetFileName returns string?, but ToDictionary needs string. We resolve this with the following, and
        // then tell the compiler to stop complaining that GetFileName(x) is never null; this is about the return type.
        // ReSharper disable once ConstantNullCoalescingCondition
        var agentsAndLocations = agents.ToDictionary(
            x => Path.GetFileName(x) ?? throw new ArgumentException("No file name for given path.")
        );

        eventClient.Fire(new AgentsDetectedEvent { AgentsAndLocations = agentsAndLocations });
    }
}
