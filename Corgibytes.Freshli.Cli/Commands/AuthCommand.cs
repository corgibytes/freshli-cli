using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class AuthCommand : BaseCommand
    {
        public AuthCommand() : base("auth", "Auth Command")
        {
            this.Handler = CommandHandler.Create<IHost, InvocationContext, AuthCommandOptions>(this.Run);
        }

        private void Run(IHost host, InvocationContext context, AuthCommandOptions options)
        {
            using IServiceScope scope = host.Services.CreateScope();

            context.Console.Out.Write(CliOutput.AuthCommand_AuthCommand_Executing_auth_command_handler);

            ICommandRunnerFactory commandRunnerFactory = scope.ServiceProvider.GetRequiredService<ICommandRunnerFactory>();
            ICommandRunner<AuthCommandOptions> runner = commandRunnerFactory.CreateAuthCommandRunner();
            runner.Run(options);
        }
    }
}
