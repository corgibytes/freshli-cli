using System.CommandLine;

namespace Corgibytes.Freshli.Cli.CommandRunners;

// ReSharper disable once UnusedTypeParameter
public interface ICommandRunner<in TCommand, in TCommandOptions> where TCommand : Command
    where TCommandOptions : CommandOptions.CommandOptions
{
    // TODO: Make this method return ValueTask<int>
    public int Run(TCommandOptions options, IConsole console);
}
