using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Options;
using Freshli;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
    public interface IOutputStrategy
    {
        OutputStrategyType Type { get; }

        void Send( IList<MetricsResult> results, IOutputFormatter formatter, ScanOptions options );
    }
}
