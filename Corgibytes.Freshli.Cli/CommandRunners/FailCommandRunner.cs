using System;
using System.CommandLine;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class FailCommandRunner : CommandRunner<FailCommand, EmptyCommandOptions>
{
    public FailCommandRunner(IServiceProvider serviceProvider, IRunner runner,
        IApplicationActivityEngine activityEngine)
        : base(serviceProvider, runner) =>
        ActivityEngine = activityEngine;

    private IApplicationActivityEngine ActivityEngine { get; }

    public override async ValueTask<int> Run(EmptyCommandOptions options, IConsole console)
    {
        await ActivityEngine.Dispatch(new ThrowExceptionActivity());
        await ActivityEngine.Wait();

        return 0;
    }
}
