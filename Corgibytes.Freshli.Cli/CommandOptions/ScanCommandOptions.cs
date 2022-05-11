using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public class ScanCommandOptions : CommandOptions
    {
        public FormatType Format { get; set; }

        public IList<OutputStrategyType> Output { get; set; } = new List<OutputStrategyType>();
        public string Path { get ; set; }

        public ScanCommandOptions()
        {
            // Output = new List<OutputStrategyType>();
        }
    }
}
