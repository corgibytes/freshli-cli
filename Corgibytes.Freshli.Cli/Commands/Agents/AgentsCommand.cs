using System.CommandLine;

namespace Corgibytes.Freshli.Cli.Commands.Agents;

public class AgentsCommand : Command
{
    public AgentsCommand() : base("agents", "Detects all of the language agents that are available for use")
    {
        var detect = new AgentsDetectCommand();
        AddCommand(detect);
        AgentsVerifyCommand verify = new();
        AddCommand(verify);
    }
}
