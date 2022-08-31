using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AnalyzeRunner : CommandRunner<AnalyzeCommand, AnalyzeCommandOptions>
{
    public AnalyzeRunner(IServiceProvider serviceProvider, Runner runner) : base(serviceProvider, runner)
    {
    }

    public override int Run(AnalyzeCommandOptions options, InvocationContext context) => throw new NotImplementedException();
}

