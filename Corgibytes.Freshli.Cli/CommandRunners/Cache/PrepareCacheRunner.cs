using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class CachePrepareCommandRunner : CommandRunner<CacheCommand, CachePrepareCommandOptions>
{
    public CachePrepareCommandRunner(IServiceProvider serviceProvider, Runner runner,
        IApplicationActivityEngine activityEngine, IConfiguration configuration)
        : base(serviceProvider, runner)
    {
        Configuration = configuration;
        ActivityEngine = activityEngine;
    }

    private IConfiguration Configuration { get; }
    private IApplicationActivityEngine ActivityEngine { get; }

    public override int Run(CachePrepareCommandOptions options, InvocationContext context)
    {
        Configuration.CacheDir = options.CacheDir;

        ActivityEngine.Dispatch(new PrepareCacheActivity(options.CacheDir));
        ActivityEngine.Wait();
        return 0;
    }
}
