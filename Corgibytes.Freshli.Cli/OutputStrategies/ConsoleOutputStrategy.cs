using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Options;
using Freshli;
using System;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
    public class ConsoleOutputStrategy : IOutputStrategy
    {
        public OutputStrategyType Type => OutputStrategyType.console;

        public void Send( IList<MetricsResult> results, IOutputFormatter formatter, ScanOptions options )
        {
            Console.WriteLine("Sending metrics to Console");
            Console.Out.WriteLine(formatter.Format(results));
        }
    }
}
