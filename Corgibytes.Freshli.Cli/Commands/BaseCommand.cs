using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Corgibytes.Freshli.Cli.Commands
{
    public abstract class BaseCommand<T> : Command where T: CommandOptions.CommandOptions
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        protected BaseCommand(string name, string description = null) : base(name, description)
        {
              Option <FormatType> formatOption = new(new[] { "--format", "-f" },
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

            AddOption(formatOption);
            AddOption(outputOption);

            Handler = CommandHandler.Create<IHost, InvocationContext, T>(Run);

        }

        private void Run(IHost host, InvocationContext context, T options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            s_logger.Info($"CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler\n");

            using IServiceScope scope = host.Services.CreateScope();
            ICommandRunner<T> runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<T>>();

            runner.Run(options);
        }
    }
}
