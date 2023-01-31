using System.Collections.Generic;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class AgentsDetectedEvent : IApplicationEvent
{
    public Dictionary<string, string>? AgentsAndLocations { get; init; }

    public ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        return ValueTask.CompletedTask;
    }
}
