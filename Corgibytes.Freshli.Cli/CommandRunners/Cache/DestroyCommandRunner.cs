using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.CacheDestroy;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class CacheDestroyCommandRunner : CommandRunner<CacheCommand, CacheDestroyCommandOptions>
{
    public CacheDestroyCommandRunner(IServiceProvider serviceProvider, Runner runner,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine)
        : base(serviceProvider, runner)
    {
        ActivityEngine = activityEngine;
        EventEngine = eventEngine;
    }

    private IApplicationActivityEngine ActivityEngine { get; }
    private IApplicationEventEngine EventEngine { get; }

    public override int Run(CacheDestroyCommandOptions options, InvocationContext context)
    {
        ActivityEngine.Dispatch(new DestroyCacheActivity(options, context));

        var isConfirmationRequired = false;
        EventEngine.On<ConfirmationRequiredEvent>(_ => { isConfirmationRequired = true; });

        var exitCode = WaitForCacheDestroyEvents(context);
        ActivityEngine.Wait();

        if (isConfirmationRequired)
        {
            var strConfirmDestroy = string.Format(
                CliOutput.CacheDestroyCommandRunner_Run_Prompt,
                options.CacheDir.FullName);
            if (!Confirm(strConfirmDestroy, context))
            {
                context.Console.Out.WriteLine(
                    CliOutput.CacheDestroyCommandRunner_Run_Abort);
                return true.ToExitCode();
            }

            ActivityEngine.Dispatch(new ForceDestroyCacheActivity(options, context));

            exitCode = WaitForCacheDestroyEvents(context);
        }

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

        EventEngine.On<CacheDestroyedEvent>(destroyEvent =>
        {
            exitCode = destroyEvent.ExitCode;
        });

        ActivityEngine.Wait();

        return exitCode;
    }
}
