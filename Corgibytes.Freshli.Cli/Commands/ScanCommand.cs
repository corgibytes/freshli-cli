using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class ScanCommand : BaseCommand
    {
        public ScanCommand() : base("scan", "Scan command returns metrics results for given local repositorry path")
        {

            Argument<DirectoryInfo> pathArgument = new("path", "Source code repository path")
            {
                Arity = ArgumentArity.ExactlyOne
            };

            this.AddArgument(pathArgument);

            this.Handler = CommandHandler.Create<IHost, ScanCommandOptions>((host, options) =>
           {
               using IServiceScope scope = host.Services.CreateScope();

               Console.WriteLine(CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler);

               ICommandRunnerFactory commandRunnerFactory = scope.ServiceProvider.GetRequiredService<ICommandRunnerFactory>();
               ICommandRunner<ScanCommandOptions> runner = commandRunnerFactory.CreateScanCommandRunner(options);
               runner.Run(options);
           });
        }
    }
}
