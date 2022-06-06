using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners
{
    public class CacheCommandRunner : CommandRunner<CacheCommandOptions>
    {
        public CacheCommandRunner(IServiceProvider serviceProvider, Runner runner)
            : base(serviceProvider, runner)
        {

        }

        public override int Run(CacheCommandOptions options, InvocationContext context)
        {
            return 0;
        }
    }
}
