using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
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

        EventEngine.On<CacheDestroyedEvent>(destroyEvent =>
        {
            // TODO
            // Should anything happen here after the CacheDestroyedEvent is
            // received?
        });

        ActivityEngine.Wait();

        return 0;
    }
}
