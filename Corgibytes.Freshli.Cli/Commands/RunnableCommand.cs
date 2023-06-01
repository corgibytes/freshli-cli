using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands;

public abstract class RunnableCommand<TCommand, TCommandOptions> : Command where TCommand : Command
    where TCommandOptions : CommandOptions.CommandOptions
{
    protected RunnableCommand(string name, string? description = null) : base(name, description)
    {
        Handler = CommandHandler.Create<IHost, IConsole, InvocationContext, TCommandOptions>(
            async (host, console, context, options) => await Run(host, console, context, options));
    }

    protected virtual async ValueTask<int> Run(IHost host, IConsole console, InvocationContext context, TCommandOptions options)
    {
        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<TCommand, TCommandOptions>>();
        var cancellationToken = context.GetCancellationToken();

        cancellationToken.Register(() => console.WriteLine("Cancellation requested. Exiting..."));

        return await runner.Run(options, console, cancellationToken);
    }
}
