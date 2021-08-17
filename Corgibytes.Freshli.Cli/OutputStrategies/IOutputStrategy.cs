using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Lib;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
    public interface IOutputStrategy
    {
        OutputStrategyType Type { get; }

        void Send( IList<MetricsResult> results, IOutputFormatter formatter, ScanCommandOptions options );
    }
}
