using System;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners
{
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
            return Cache.Prepare(options.CacheDir).ToExitCode();
        }
    }

    public class CacheDestroyCommandRunner : CommandRunner<CacheDestroyCommandOptions>
    {
        public CacheDestroyCommandRunner(IServiceProvider serviceProvider, Runner runner)
            : base(serviceProvider, runner)
        {

        }

        public override int Run(CacheDestroyCommandOptions options)
        {
            // Skip prompt if the --force flag is passed
            if (!options.Force)
            {
                // Prompt the user whether they want to destroy the cache
                Console.Out.Write($"Do you want to destroy the Freshli cache at {options.CacheDir.FullName}? [y/N] ");
                string choice = Console.In.ReadLine();
                string[] choicesToProceed = {"y", "Y"};
                // If a "proceed" choice is not input, abort operation.
                if (!choicesToProceed.Contains(choice))
                {
                    Console.Out.WriteLine("Operation aborted. Cache not destroyed.");
                    return 0;
                }
            }

            // Destroy the cache
            Console.Out.WriteLine("Destroying cache...");
            return Cache.Destroy(options.CacheDir).ToExitCode();
        }
    }
}
