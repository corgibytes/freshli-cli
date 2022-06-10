using System;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners
{
    public class AgentsCommandRunner : CommandRunner<AgentsCommandOptions>
    {
        public AgentsCommandRunner(IServiceProvider serviceProvider, Runner runner)
            : base(serviceProvider, runner)
        {
            
        }

        public override int Run(AgentsCommandOptions options)
        {
            return 0;
        }
    }

    public class AgentsDetectCommandRunner : CommandRunner<AgentsDetectCommandOptions>
    {
        public AgentsDetectCommandRunner(IServiceProvider serviceProvider, Runner runner)
            : base(serviceProvider, runner)
        {
            
        }

        public override int Run(AgentsDetectCommandOptions options)
        {
            bool success = Cache.Prepare(options.CacheDir);
            return success ? 0 : 1;
        }
    }
}

