using System;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class CacheCommandRunner : CommandRunner<CacheCommandOptions>
{
    public CacheCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {

    }

    public override int Run(CacheCommandOptions options)
    {
        return 0;
    }
}

public class CachePrepareCommandRunner : CommandRunner<CachePrepareCommandOptions>
{
    public CachePrepareCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {

    }

    public override int Run(CachePrepareCommandOptions options)
    {
        var success = Cache.Prepare(options.CacheDir);
        return success ? 0 : 1;
    }
}

