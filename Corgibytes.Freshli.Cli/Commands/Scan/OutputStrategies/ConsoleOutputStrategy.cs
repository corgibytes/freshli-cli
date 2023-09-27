using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands.Scan.Formatters;
using Corgibytes.Freshli.Lib;
using NLog;

namespace Corgibytes.Freshli.Cli.Commands.Scan.OutputStrategies;

public class ConsoleOutputStrategy : IOutputStrategy
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public OutputStrategyType Type => OutputStrategyType.Console;

    public void Send(IList<ScanResult> results, IOutputFormatter formatter, ScanCommandOptions options)
    {
        // ReSharper disable once LocalizableElement
        _logger.Info("Sending metrics to Console");
        Console.Out.WriteLine(formatter.Format(results));
    }
}
