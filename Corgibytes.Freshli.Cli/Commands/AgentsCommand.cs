using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsCommand : Command
{
    public AgentsCommand() : base("agents", "Detects all of the language agents that are available for use")
    {
        var detect = new AgentsDetectCommand();
        AddCommand(detect);
    }
}

public class AgentsDetectCommand : RunnableCommand<AgentsDetectCommand, EmptyCommandOptions>
{
    public AgentsDetectCommand() : base("detect",
        "Outputs the detected language name and the path to the language agent binary in a tabular format")
    {
    }
}
