using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class AgentsDetectedEvent : IApplicationEvent
{
    public Dictionary<string, string>? AgentsAndLocations { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
