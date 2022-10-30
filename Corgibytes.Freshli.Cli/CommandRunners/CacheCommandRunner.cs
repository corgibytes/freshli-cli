using System;
using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class CacheCommandRunner : CommandRunner<CacheCommand, CacheCommandOptions>
{
    public CacheCommandRunner(IServiceProvider serviceProvider, IRunner runner)
        : base(serviceProvider, runner)
    {
    }

    public override int Run(CacheCommandOptions options, IConsole console) => 0;
}
