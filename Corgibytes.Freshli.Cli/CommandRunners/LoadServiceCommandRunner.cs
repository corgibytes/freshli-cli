using System;
using System.CommandLine;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class LoadServiceCommandRunner : CommandRunner<LoadServiceCommand, EmptyCommandOptions>
{
    public LoadServiceCommandRunner(IServiceProvider serviceProvider, IRunner runner,
        IApplicationActivityEngine activityEngine)
        : base(serviceProvider, runner) =>
        ActivityEngine = activityEngine;

    private IApplicationActivityEngine ActivityEngine { get; }

    public override async ValueTask<int> Run(EmptyCommandOptions options, IConsole console)
    {
        var activity = new LoadServiceProviderActivity();
        await ActivityEngine.Dispatch(activity);
        await ActivityEngine.Wait(activity);
        return 0;
    }
}
