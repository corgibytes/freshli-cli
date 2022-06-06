using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.OutputStrategies;

public class ConsoleOutputStrategy : IOutputStrategy
{
    public OutputStrategyType Type => OutputStrategyType.Console;

    public virtual void Send(IList<ScanResult> results, IOutputFormatter formatter, ScanCommandOptions options)
    {
        Console.WriteLine("Sending metrics to Console");
        Console.Out.WriteLine(formatter.Format(results));
    }
}
