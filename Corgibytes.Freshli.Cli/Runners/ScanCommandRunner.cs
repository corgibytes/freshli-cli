using Autofac.Extras.DynamicProxy;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.IoC.Interceptors;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Freshli;
using NLog;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Runners
{
    [Intercept(typeof(LoggerInterceptor))]
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

        public virtual int Run( ScanOptions options )
        {
            var results = this.Runner.Run(options.Path);

            foreach(IOutputStrategy output in this.OutputStrategies)
            {
                output.Send(results, this.OutputFormatter, options);
            }

            return 0;
        }
    }
}
