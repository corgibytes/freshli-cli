using System.Collections.Generic;
using System.CommandLine;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;

namespace Corgibytes.Freshli.Cli.Commands
{
    public abstract class BaseCommand : Command
    {
        protected BaseCommand(string name, string description = null) : base(name, description)
        {
            Option<FormatType> formatOption = new(new[] { "--format", "-f" },
                description: "Represents the output format type - It's value is case insensitive",
                getDefaultValue: () => FormatType.Json)
            {
                AllowMultipleArgumentsPerToken = false,
                Arity = ArgumentArity.ExactlyOne,
            };

            Option<IEnumerable<OutputStrategyType>> outputOption = new(new[] { "--output", "-o" },
                description: "Represents where you want to output the result. This option is case sensitive and you can specify more than one by including it multiple times. Allowed values are [ console | file ]",
                getDefaultValue: () => new List<OutputStrategyType>() { OutputStrategyType.Console })
            {
                AllowMultipleArgumentsPerToken = true,
                Arity = ArgumentArity.OneOrMore,
            };

            this.AddOption(formatOption);
            this.AddOption(outputOption);

        }
    }
}
