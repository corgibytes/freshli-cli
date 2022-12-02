using System.Collections.Generic;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class AgentsDetectedEvent : ApplicationEventBase
{
    public Dictionary<string, string>? AgentsAndLocations { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        return ValueTask.CompletedTask;
    }
}
