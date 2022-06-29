using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class CheckoutHistoryCommandRunner : CommandRunner<CheckoutHistoryCommandOptions>
{
    public CheckoutHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner) : base(serviceProvider, runner)
    {
    }

    public override int Run(CheckoutHistoryCommandOptions options, InvocationContext context)
    {
        return 0;
    }
}

