using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public abstract class CommandOptions : ICommandOption
    {
        public abstract CommandOptionType Type { get; }
        public FormatType Format { get; set; }
        public IList<OutputStrategyType> Output { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
