using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.OutputStrategies;

//    [Intercept(typeof(LoggerInterceptor))]
public class ConsoleOutputStrategy : IOutputStrategy
{
    public OutputStrategyType Type => OutputStrategyType.Console;

    public void Send(IList<ScanResult> results, IOutputFormatter formatter, ScanCommandOptions options)
    {
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Sending metrics to Console");
        Console.Out.WriteLine(formatter.Format(results));
    }
}
