using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.OutputStrategies;
using System.Collections.Generic;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners
{

    public class ScanCommandRunner : ICommandRunner<ScanCommandOptions>
    {        
        public Runner Runner { get; set; }
        public IList<IOutputStrategy> OutputStrategies { get; set; }
        public IOutputFormatter OutputFormatter { get; set; }

        public ScanCommandRunner( IList<IOutputStrategy> outputStrategy, IOutputFormatter outputFormatter, Runner runner )
        {
            this.OutputStrategies = outputStrategy;
            this.OutputFormatter = outputFormatter;
            this.Runner = runner;
        }
        
        public virtual int Run( ScanCommandOptions options )
        {
            IList<MetricsResult> results = this.Runner.Run(options.Path);

            foreach(IOutputStrategy output in this.OutputStrategies)
            {
                output.Send(results, this.OutputFormatter, options);
            }

            return 0;
        }
    }
}
