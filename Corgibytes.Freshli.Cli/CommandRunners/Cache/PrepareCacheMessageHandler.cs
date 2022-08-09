using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class PrepareCacheMessageHandler : CommandRunner<CacheCommand, CachePrepareCommandOptions>
{
    public PrepareCacheMessageHandler(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {
    }

    public override int Run(PrepareCacheMessageHandler options, InvocationContext context)
    {
        context.Console.Out.WriteLine(
            string.Format(CliOutput.PrepareCacheMessageHandler_Run_Preparing_cache, options.CacheDir)
        );
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
