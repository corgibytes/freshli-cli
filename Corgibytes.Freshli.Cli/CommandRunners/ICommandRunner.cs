using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.CommandRunners;

// ReSharper disable once UnusedTypeParameter
public interface ICommandRunner<in TCommand, in TCommandOptions> where TCommand : Command
    where TCommandOptions : CommandOptions.CommandOptions
{
    public ValueTask<int> Run(TCommandOptions options, IConsole console, CancellationToken context);
}
