using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public abstract class CommandOptions
    {        
        public FormatType Format { get; set; }
        public IList<OutputStrategyType> Output { get; set; }

        protected CommandOptions()
        {
            Output = new List<OutputStrategyType>();
        }
    }
}
