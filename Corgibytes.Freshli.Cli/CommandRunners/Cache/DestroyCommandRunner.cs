using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.CacheDestroy;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class CacheDestroyCommandRunner : CommandRunner<CacheCommand, CacheDestroyCommandOptions>
{
    public CacheDestroyCommandRunner(IServiceProvider serviceProvider, Runner runner, ICacheManager cacheManager,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine)
        : base(serviceProvider, runner)
    {
        CacheManager = cacheManager;
        ActivityEngine = activityEngine;
        EventEngine = eventEngine;
    }

    private ICacheManager CacheManager { get; }
    private IApplicationActivityEngine ActivityEngine { get; }
    private IApplicationEventEngine EventEngine { get; }

    public override int Run(CacheDestroyCommandOptions options, InvocationContext context)
    {
        var strConfirmDestroy = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Prompt,
            options.CacheDir);

        // Unless the --force flag is passed, prompt the user whether they want
        // to destroy the cache
        if (!options.Force && !Confirm(strConfirmDestroy, context))
        {
            context.Console.Out.WriteLine(
                CliOutput.CacheDestroyCommandRunner_Run_Abort);
            return true.ToExitCode();
        }

        var strDestroyingCache = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Destroying,
            options.CacheDir);
        context.Console.Out.WriteLine(strDestroyingCache);

        ActivityEngine.Dispatch(new DestroyCacheActivity(CacheManager, options.CacheDir));

        var exitCode = WaitForCacheDestroyEvents(context);
        return exitCode;
    }

    private int WaitForCacheDestroyEvents(InvocationContext context)
    {
        var exitCode = true.ToExitCode();

        EventEngine.On<CacheDestroyFailedEvent>(destroyEvent =>
        {
            context.Console.Error.WriteLine(destroyEvent.ResultMessage);
            exitCode = false.ToExitCode();
        });

        EventEngine.On<CacheDestroyedEvent>(destroyEvent => { exitCode = destroyEvent.ExitCode; });

        ActivityEngine.Wait();

        return exitCode;
    }
}
