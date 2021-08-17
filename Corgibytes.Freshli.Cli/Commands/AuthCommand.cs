using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class AuthCommand : BaseCommand
    {
        public AuthCommand() : base("auth", "Auth Command")
        {
            this.Handler = CommandHandler.Create<IHost, AuthCommandOptions>((host, options) =>
            {
                using IServiceScope scope = host.Services.CreateScope();

                Console.WriteLine("Executing auth command handler");

                ICommandRunnerFactory commandRunnerFactory = scope.ServiceProvider.GetRequiredService<ICommandRunnerFactory>();
                ICommandRunner<AuthCommandOptions> runner = commandRunnerFactory.CreateAuthCommandRunner();
                runner.Run(options);
            });
        }
    }
}
