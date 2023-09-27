namespace Corgibytes.Freshli.Cli.Commands.Fail;

public class FailCommand : RunnableCommand<FailCommand, EmptyCommandOptions>
{
    public FailCommand() : base("fail", "Simulates failure in an application activity")
    {
    }
}
