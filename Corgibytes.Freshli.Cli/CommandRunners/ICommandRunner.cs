using System.CommandLine;
using System.CommandLine.Invocation;

namespace Corgibytes.Freshli.Cli.CommandRunners;

// ReSharper disable once UnusedTypeParameter
public interface ICommandRunner<in TCommand, in TCommandOptions> where TCommand : Command
    where TCommandOptions : CommandOptions.CommandOptions
{
    public int Run(TCommandOptions options, InvocationContext context);
}
