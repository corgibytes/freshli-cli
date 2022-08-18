using System;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class DetectAgentsActivity : IApplicationActivity
{
    [JsonProperty] private readonly IAgentsDetector _agentsDetector;

    public DetectAgentsActivity(IAgentsDetector agentsDetector) => _agentsDetector = agentsDetector;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agents = _agentsDetector.Detect();

        // Path.GetFileName returns string?, but ToDictionary needs string. We resolve this with the following, and
        // then tell the compiler to stop complaining that GetFileName(x) is never null; this is about the return type.
        // ReSharper disable once ConstantNullCoalescingCondition
        var agentsAndLocations = agents.ToDictionary(
            x => Path.GetFileName(x) ?? throw new ArgumentException("No file name for given path.")
        );

        eventClient.Fire(new AgentsDetectedEvent { AgentsAndLocations = agentsAndLocations });
    }
}
