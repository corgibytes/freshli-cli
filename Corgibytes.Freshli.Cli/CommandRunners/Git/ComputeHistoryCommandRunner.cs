using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class ComputeHistoryCommandRunner : CommandRunner<ComputeHistoryCommand, ComputeHistoryCommandOptions>
{
    public ComputeHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner) : base(serviceProvider, runner)
    {
    }

    public override int Run(ComputeHistoryCommandOptions options, InvocationContext context) => 0;
}
