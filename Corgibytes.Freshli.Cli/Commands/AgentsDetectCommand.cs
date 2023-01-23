using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsDetectCommand : RunnableCommand<AgentsDetectCommand, EmptyCommandOptions>
{
    public AgentsDetectCommand() : base("detect",
        "Outputs the detected language name and the path to the language agent binary in a tabular format")
    {
    }
}
