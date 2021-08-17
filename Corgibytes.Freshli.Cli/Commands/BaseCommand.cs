using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Corgibytes.Freshli.Cli.Commands
{
    public abstract class BaseCommand: Command
    {
        public IoCCommandRunnerFactory CommandRunnerFactory { get; set; }

        public BaseCommand(string name, string description = null) : base(name, description) {        
            Option<FormatType> formatOption = new (new[] { "--format", "-f" },
                description: "Represents the output format type - It's value is case insensitive",
                getDefaultValue: () => FormatType.Json)
            {
                AllowMultipleArgumentsPerToken = false,
                Arity = ArgumentArity.ZeroOrOne,
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


            this.Handler = CommandHandler.Create<IHost>((host) =>
            {
                using IServiceScope scope = host.Services.CreateScope();

                var services = scope.ServiceProvider;
                var invocationContext = services.GetRequiredService<InvocationContext>();
                var bindingContext = services.GetRequiredService<BindingContext>();
                var parseResult = services.GetRequiredService<ParseResult>();
                var console = services.GetRequiredService<IConsole>();
            });
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
