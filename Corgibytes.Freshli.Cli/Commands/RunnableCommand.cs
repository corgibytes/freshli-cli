using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands;

#nullable enable
public abstract class RunnableCommand<TCommand, TCommandOptions> : Command where TCommand : Command where TCommandOptions : CommandOptions.CommandOptions
{
    protected RunnableCommand(string name, string? description = null) : base(name, description)
    {
        Handler = CommandHandler.Create<IHost, InvocationContext, TCommandOptions>(Run);
    }

    protected virtual int Run(IHost host, InvocationContext context, TCommandOptions options)
    {
        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<TCommand, TCommandOptions>>();
        return runner.Run(options, context);
    }
}
#nullable restore
