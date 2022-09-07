using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class LoadServiceCommand : RunnableCommand<LoadServiceCommand, EmptyCommandOptions>
{
    public LoadServiceCommand() : base("load-service", "Loads a service to test if the dependency injection is working accordingly.")
    {
    }
}
