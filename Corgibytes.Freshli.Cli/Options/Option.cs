using CommandLine;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Options
{
    public abstract class Option : IOption
    {
        public abstract OptionType Type { get; }

        [Option('f', "format", Default = FormatType.json, Required = false, HelpText = "[ csv | json | yaml ] - This option is case sensitive")]
        public FormatType Format { get; set; }

        [Option('o', "output", Default = new[] { OutputStrategyType.console }, Separator = ',', Required = false, HelpText = "[ console | file ] - This option is case sensitive")]
        public IList<OutputStrategyType> Output { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
