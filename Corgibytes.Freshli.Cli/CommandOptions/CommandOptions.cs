using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public abstract class CommandOptions : ICommandOption
    {
        public abstract CommandOptionType Type { get; }
        public FormatType Format { get; set; }
        public IList<OutputStrategyType> Output { get; set; }

        protected CommandOptions()
        {
            this.Output = new List<OutputStrategyType>();
        }
    }
}
