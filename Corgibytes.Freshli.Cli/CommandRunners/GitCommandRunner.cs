using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class GitCommandRunner : CommandRunner<GitCommandOptions>
{
    public GitCommandRunner(IServiceProvider serviceProvider, Runner runner) : base(serviceProvider, runner)
    {
    }

    public override int Run(GitCommandOptions options, InvocationContext context)
    {
        return 0;
    }
}

