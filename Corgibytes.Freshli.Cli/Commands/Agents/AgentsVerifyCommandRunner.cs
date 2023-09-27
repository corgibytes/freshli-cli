using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Agents;

public class AgentsVerifyCommandRunner : CommandRunner<AgentsVerifyCommand, AgentsVerifyCommandOptions>
{
    public AgentsVerifyCommandRunner(IServiceProvider serviceProvider, IRunner runner)
        : base(serviceProvider, runner)
    {
    }

    public override async ValueTask<int> Run(AgentsVerifyCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        // TODO: Implement this method by dispatching an activity
        await Console.Out.WriteLineAsync("This is not yet implemented.");
        return 0;
    }
}
