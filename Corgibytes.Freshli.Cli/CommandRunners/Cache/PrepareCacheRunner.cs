using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class CachePrepareCommandRunner : CommandRunner<CacheCommand, CachePrepareCommandOptions>
{
    public CachePrepareCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {
    }

    public override int Run(CachePrepareCommandOptions options, InvocationContext context)
    {
        context.Console.Out.WriteLine($"Preparing cache at {options.CacheDir}");
        try
        {
            return Functionality.Cache.Prepare(options.CacheDir).ToExitCode();
        }
        catch (CacheException e)
        {
            context.Console.Error.WriteLine(e.Message);
            return false.ToExitCode();
        }
    }
}
