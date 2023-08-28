using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsVerifyCommandRunner : CommandRunner<AgentsVerifyCommand, AgentsVerifyCommandOptions>
{
    private readonly IAgentsDetector _agentsDetector;

    public AgentsVerifyCommandRunner(IServiceProvider serviceProvider, IRunner runner, AgentsVerifier agentsVerifier,
        IAgentsDetector agentsDetector)
        : base(serviceProvider, runner)
    {
        _agentsDetector = agentsDetector;
        AgentsVerifier = agentsVerifier;
    }

    private AgentsVerifier AgentsVerifier { get; }

    public override async ValueTask<int> Run(AgentsVerifyCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        // TODO: Implement this method by dispatching an activity
        await Console.Out.WriteLineAsync("This is not yet implemented.");
        return 0;
    }
}
