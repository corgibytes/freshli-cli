using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Commands;

// ReSharper disable once UnusedTypeParameter
public interface ICommandRunner<in TCommand, in TCommandOptions> where TCommand : Command
    where TCommandOptions : CommandOptions
{
    public ValueTask<int> Run(TCommandOptions options, IConsole console, CancellationToken context);
}
