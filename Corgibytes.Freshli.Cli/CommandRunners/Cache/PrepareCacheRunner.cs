using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class CachePrepareCommandRunner : CommandRunner<CacheCommand, CachePrepareCommandOptions>
{
    public CachePrepareCommandRunner(IServiceProvider serviceProvider, ICacheManager cacheManager, Runner runner,
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

    public override int Run(CachePrepareCommandOptions options, InvocationContext context)
    {
        ActivityEngine.Dispatch(new PrepareCacheActivity(options.CacheDir));
        ActivityEngine.Wait();
        return 0;

    }
}
