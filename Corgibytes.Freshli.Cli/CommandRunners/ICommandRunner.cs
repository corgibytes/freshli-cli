using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public interface ICommandRunner<T> where T : CommandOptions.CommandOptions
{
    public int Run(T options);
}
