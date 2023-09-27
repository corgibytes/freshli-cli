using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands.Scan.Formatters;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Scan.OutputStrategies;

public interface IOutputStrategy
{
    OutputStrategyType Type { get; }

    void Send(IList<ScanResult> results, IOutputFormatter formatter, ScanCommandOptions options);
}
