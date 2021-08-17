using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class ScanCommand : BaseCommand
    {
        public ScanCommand() : base("scan", "Scan command returns metrics results for given local repository path")
        {

            Argument<DirectoryInfo> pathArgument = new("path", "Source code repository path")
            {
                Arity = ArgumentArity.ExactlyOne
            };

            this.AddArgument(pathArgument);

            this.Handler = CommandHandler.Create<IHost, InvocationContext, ScanCommandOptions >(this.Run);
        }

        private void Run(IHost host, InvocationContext context, ScanCommandOptions options)
        {
            using IServiceScope scope = host.Services.CreateScope();

            context.Console.Out.Write($"CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler\n");

            ICommandRunnerFactory commandRunnerFactory = scope.ServiceProvider.GetRequiredService<ICommandRunnerFactory>();
            ICommandRunner<ScanCommandOptions> runner = commandRunnerFactory.CreateScanCommandRunner(options);
            runner.Run(options);
        }
    }
}
