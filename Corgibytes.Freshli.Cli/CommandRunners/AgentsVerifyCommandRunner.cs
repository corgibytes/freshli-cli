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

    // TODO: This method should dispatch an activity
    public override async ValueTask<int> Run(AgentsVerifyCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        var agents = _agentsDetector.Detect();

        if (options.LanguageName == "")
        {
            foreach (var agentsAndPath in agents)
            {
                await AgentsVerifier.RunAgentsVerify(agentsAndPath, "validating-repositories", options.CacheDir, "");
            }
        }
        else
        {
            foreach (var agentsAndPath in agents)
            {
                if (agentsAndPath.ToLower().Contains("freshli-agent-" + options.LanguageName.ToLower()))
                {
                    await AgentsVerifier.RunAgentsVerify(agentsAndPath, "validating-repositories", options.CacheDir,
                        options.LanguageName);
                }
            }
        }

        return 0;
    }
}
