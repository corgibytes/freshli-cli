using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
    public interface IOutputStrategy
    {
        OutputStrategyType Type { get; }

        void Send(IList<ScanResult> results, IOutputFormatter formatter, ScanCommandOptions options);
    }
}
