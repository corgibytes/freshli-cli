using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Freshli;
using NLog;
using System;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Runners
{
    public class ScanCommandRunner : ICommandRunner<ScanOptions>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public Runner Runner { get; set; }
        public IList<IOutputStrategy> OutputStrategies { get; set; }
        public IOutputFormatter OutputFormatter { get; set; }

        public delegate ScanCommandRunner Factory( IList<IOutputStrategy> outputStrategy, IOutputFormatter outputFormatter );

        public ScanCommandRunner( IList<IOutputStrategy> outputStrategy, IOutputFormatter outputFormatter, Runner runner )
        {
            this.OutputStrategies = outputStrategy;
            this.OutputFormatter = outputFormatter;
            this.Runner = runner;
        }

        public int Run( ScanOptions options )
        {
            try
            {

                var path = options.Path;
                logger.Debug($"Collecting data for path = { options.Path }, format = { options.Format }, output = { string.Join(",", options.Output) }");
                var results = this.Runner.Run(options.Path);

                foreach(IOutputStrategy output in this.OutputStrategies)
                {
                    output.Send(results, this.OutputFormatter, options);
                }

                return 0;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception executing Freshli for args path = { options.Path }, format = { options.Format }, output = { options.Output } : { e.Message} : {e.StackTrace}");
                logger.Error(
                  e,
                  $"Exception executing Freshli for args path={options.Path}, format={options.Format}, output={options.Output} : {e.Message}"
                );
                logger.Trace(e, e.StackTrace);
                throw;
            }
        }
    }
}
