using System;
using Corgibytes.Freshli.Cli.CommandOptions;
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
            bool success = Cache.Prepare(options.CacheDir);
            return success ? 0 : 1;
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
            bool success = Cache.Destroy(options.CacheDir);
            return (success == true ? 0 : 1);
        }
    }
}

