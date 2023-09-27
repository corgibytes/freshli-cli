using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class CacheDestroyCommandRunner : CommandRunner<CacheCommand, CacheDestroyCommandOptions>
{
    public CacheDestroyCommandRunner(IServiceProvider serviceProvider, IRunner runner,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine, IConfiguration configuration)
        : base(serviceProvider, runner)
    {
        Configuration = configuration;
        ActivityEngine = activityEngine;
        EventEngine = eventEngine;
    }

    private IConfiguration Configuration { get; }
    private IApplicationActivityEngine ActivityEngine { get; }
    private IApplicationEventEngine EventEngine { get; }

    public override async ValueTask<int> Run(CacheDestroyCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        Configuration.CacheDir = options.CacheDir;

        var strConfirmDestroy = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Prompt,
            options.CacheDir);

        // Unless the --force flag is passed, prompt the user whether they want
        // to destroy the cache
        if (!options.Force && !Confirm(strConfirmDestroy, console))
        {
            console.Out.WriteLine(
                CliOutput.CacheDestroyCommandRunner_Run_Abort);
            return true.ToExitCode();
        }

        var strDestroyingCache = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Destroying,
            options.CacheDir);
        console.Out.WriteLine(strDestroyingCache);

        var activity = new DestroyCacheActivity();

        var exitCode = true.ToExitCode();

        EventEngine.On<CacheDestroyFailedEvent>(destroyEvent =>
        {
            console.Error.WriteLine(destroyEvent.ResultMessage);
            exitCode = false.ToExitCode();
            return ValueTask.CompletedTask;
        });

        EventEngine.On<CacheDestroyedEvent>(destroyEvent =>
        {
            exitCode = destroyEvent.ExitCode;
            return ValueTask.CompletedTask;
        });

        await ActivityEngine.Dispatch(activity, cancellationToken);
        await ActivityEngine.Wait(activity, cancellationToken);
        return exitCode;
    }
}
