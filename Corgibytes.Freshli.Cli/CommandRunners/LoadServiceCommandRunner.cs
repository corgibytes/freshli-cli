using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class LoadServiceCommandRunner : CommandRunner<LoadServiceCommand, EmptyCommandOptions>
{
    public LoadServiceCommandRunner(IServiceProvider serviceProvider, Runner runner,
        IApplicationActivityEngine activityEngine)
        : base(serviceProvider, runner) =>
        ActivityEngine = activityEngine;

    private IApplicationActivityEngine ActivityEngine { get; }

    public override int Run(EmptyCommandOptions options, InvocationContext context)
    {
        ActivityEngine.Dispatch(new LoadServiceProviderActivity());
        ActivityEngine.Wait();
        return 0;
    }
}
