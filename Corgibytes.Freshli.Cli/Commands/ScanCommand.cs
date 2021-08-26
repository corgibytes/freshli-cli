using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class ScanCommand : BaseCommand
    {
        public ScanCommand() : base("scan", "Scan command returns metrics results for given local repository path")
        {

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
