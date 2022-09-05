using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class FailCommand : RunnableCommand<FailCommand, EmptyCommandOptions>
{
    public FailCommand() : base("fail", "Simulates failure in an application activity")
    {
    }
}
