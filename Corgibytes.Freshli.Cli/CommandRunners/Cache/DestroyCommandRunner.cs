using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class CacheDestroyCommandRunner : CommandRunner<CacheCommand, CacheDestroyCommandOptions>
{
    private ICacheManager CacheManager { get; }

    public CacheDestroyCommandRunner(IServiceProvider serviceProvider, ICacheManager cacheManager, Runner runner)
        : base(serviceProvider, runner)
    {
        CacheManager = cacheManager;
    }

    public override int Run(CacheDestroyCommandOptions options, InvocationContext context)
    {
        // Unless the --force flag is passed, prompt the user whether they want to destroy the cache
        if (!options.Force && !Confirm(
                string.Format(CliOutput.CacheDestroyCommandRunner_Run_Prompt, options.CacheDir),
                context
            ))
        {
            context.Console.Out.WriteLine(CliOutput.CacheDestroyCommandRunner_Run_Abort);
            return true.ToExitCode();
        }

        // Destroy the cache
        context.Console.Out.WriteLine(string.Format(CliOutput.CacheDestroyCommandRunner_Run_Destroying,
            options.CacheDir));
        try
        {
            return CacheManager.Destroy(options.CacheDir).ToExitCode();
        }
        // Catch errors
        catch (CacheException error)
        {
            context.Console.Error.WriteLine(error.Message);
            return error.IsWarning.ToExitCode();
        }
    }
}
