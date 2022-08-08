using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using Elasticsearch.Net;

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

        bool isConfirmationRequired = false;
        EventEngine.On<ConfirmationRequiredEvent>(_ => { isConfirmationRequired = true; });

        int exitCode = 0;
        exitCode = WaitForCacheDestroyEvents(context);
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
        int exitCode = 0;

        EventEngine.On<CacheDestroyFailedEvent>(destroyEvent =>
        {
            context.Console.Error.WriteLine(destroyEvent.ResultMessage);
            exitCode = false.ToExitCode();
        });

        EventEngine.On<CacheDestroyedEvent>(_ => { });

        ActivityEngine.Wait();

        return exitCode;
    }
}
