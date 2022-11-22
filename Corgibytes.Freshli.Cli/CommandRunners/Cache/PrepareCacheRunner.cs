using System;
using System.CommandLine;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

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

    public override async ValueTask<int> Run(CachePrepareCommandOptions options, IConsole console)
    {
        Configuration.CacheDir = options.CacheDir;

        await ActivityEngine.Dispatch(new PrepareCacheActivity());
        await ActivityEngine.Wait();
        return 0;
    }
}
