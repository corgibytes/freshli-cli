using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands;

#nullable enable
public abstract class RunnableCommand<T> : Command where T: CommandOptions.CommandOptions
{
    protected RunnableCommand(string name, string? description = null) : base(name, description)
    {
        Handler = CommandHandler.Create<IHost, InvocationContext, T>(Run);
    }

    protected virtual int Run(IHost host, InvocationContext context, T options)
    {
        using IServiceScope scope = host.Services.CreateScope();
        ICommandRunner<T> runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<T>>();
        return runner.Run(options, context);
    }
}
#nullable restore
