using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.CommandOptions;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Corgibytes.Freshli.Cli.CommandRunners;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class ScanCommand : BaseCommand
    {
        public ScanCommand() : base("scan", "Scan command returns metrics results for given local repositorry path") {

            Argument<string> pathArgument = new("path") {
                Description = "Source code repository path",
            };

            this.AddArgument(pathArgument);

            this.Handler = CommandHandler.Create<IHost, ScanCommandOptions> ((host, options) =>
            {                
                using IServiceScope scope = host.Services.CreateScope();

                Console.WriteLine("Executing scan command handler");

                ICommandRunnerFactory commandRunnerFactory = scope.ServiceProvider.GetRequiredService<ICommandRunnerFactory>();
                ICommandRunner<ScanCommandOptions> runner = commandRunnerFactory.CreateScanCommandRunner(options);
                runner.Run(options);
            });
        }     
    }
}
