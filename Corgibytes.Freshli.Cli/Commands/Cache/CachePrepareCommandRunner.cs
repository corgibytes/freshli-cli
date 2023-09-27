using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class CachePrepareCommandRunner : CommandRunner<CacheCommand, CachePrepareCommandOptions>
{
    public CachePrepareCommandRunner(IServiceProvider serviceProvider, IRunner runner,
        IApplicationActivityEngine activityEngine, IConfiguration configuration)
        : base(serviceProvider, runner)
    {
        Configuration = configuration;
        ActivityEngine = activityEngine;
    }

    private IConfiguration Configuration { get; }
    private IApplicationActivityEngine ActivityEngine { get; }

    public override async ValueTask<int> Run(CachePrepareCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        Configuration.CacheDir = options.CacheDir;

        var activity = new PrepareCacheActivity();
        await ActivityEngine.Dispatch(activity, cancellationToken);
        await ActivityEngine.Wait(activity, cancellationToken);
        return 0;
    }
}
