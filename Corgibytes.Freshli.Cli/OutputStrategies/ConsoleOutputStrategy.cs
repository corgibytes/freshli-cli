using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Lib;
using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
//    [Intercept(typeof(LoggerInterceptor))]
    public class ConsoleOutputStrategy : IOutputStrategy
    {
        public OutputStrategyType Type => OutputStrategyType.Console;
        public ConsoleOutputStrategy() { }
        
        public virtual void Send( IList<MetricsResult> results, IOutputFormatter formatter, ScanCommandOptions options )
        {
            Console.WriteLine("Sending metrics to Console");
            Console.Out.WriteLine(formatter.Format(results));
        }
    }
}
