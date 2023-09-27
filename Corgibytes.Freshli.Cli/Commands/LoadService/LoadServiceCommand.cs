namespace Corgibytes.Freshli.Cli.Commands.LoadService;

public class LoadServiceCommand : RunnableCommand<LoadServiceCommand, EmptyCommandOptions>
{
    public LoadServiceCommand() : base("load-service",
        "Loads a service to test if the dependency injection is working accordingly.")
    {
    }
}
