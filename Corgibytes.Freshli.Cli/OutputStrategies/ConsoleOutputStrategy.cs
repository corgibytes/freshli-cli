using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Lib;
using NLog;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
    //    [Intercept(typeof(LoggerInterceptor))]
    public class ConsoleOutputStrategy : IOutputStrategy
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public OutputStrategyType Type => OutputStrategyType.Console;
        public ConsoleOutputStrategy() { }

        public virtual void Send(IList<ScanResult> results, IOutputFormatter formatter, ScanCommandOptions options)
        {
            s_logger.Info("Sending metrics to Console");
            Console.Out.WriteLine(formatter.Format(results));
        }
    }
}
