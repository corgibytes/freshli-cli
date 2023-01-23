using System;
using System.CommandLine;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsCommandRunner : CommandRunner<AgentsCommand, EmptyCommandOptions>
{
    public AgentsCommandRunner(IServiceProvider serviceProvider, IRunner runner)
        : base(serviceProvider, runner)
    {
    }

    public override ValueTask<int> Run(EmptyCommandOptions options, IConsole console) => ValueTask.FromResult(0);
}
