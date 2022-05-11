using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class ScanCommand : Command
    {
        public ScanCommand() : base("scan", "Scan command returns metrics results for given local repository path")
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

            Argument<string> pathArgument = new("path", "Source code repository path")
            {
                Arity = ArgumentArity.ExactlyOne
            };

            AddArgument(pathArgument);

            Handler = CommandHandler.Create<IHost, InvocationContext, ScanCommandOptions>(Run);
        }

        private void Run(IHost host, InvocationContext context, ScanCommandOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            context.Console.Out.Write($"CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler\n");

            using IServiceScope scope = host.Services.CreateScope();
            ICommandRunner<ScanCommandOptions> runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<ScanCommandOptions>>();

            runner.Run(options);
        }
    }
}
