using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class CacheCommand : Command
    {
        public CacheCommand() : base("cache", "Manages the local cache database")
        {
            CachePrepareCommand prepare = new();
            AddCommand(prepare);

            CacheDestroyCommand destroy = new();
            AddCommand(destroy);
        }
    }

    public class CachePrepareCommand : Command
    {
        public CachePrepareCommand() : base("prepare",
            "Ensures the cache directory exists and contains a valid cache database.")
        {
            Handler = CommandHandler.Create<IHost, InvocationContext, CachePrepareCommandOptions>(Run);
        }

        private int Run(IHost host, InvocationContext context, CachePrepareCommandOptions options)
        {
            using IServiceScope scope = host.Services.CreateScope();
            ICommandRunner<CachePrepareCommandOptions> runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<CachePrepareCommandOptions>>();
            return runner.Run(options);
        }
    }

    public class CacheDestroyCommand : Command
    {
        public CacheDestroyCommand() : base("destroy",
            "Deletes the Freshli cache.")
        {
            Handler = CommandHandler.Create<IHost, InvocationContext, CacheDestroyCommandOptions>(Run);
        }

        private int Run(IHost host, InvocationContext context, CacheDestroyCommandOptions options)
        {
            using IServiceScope scope = host.Services.CreateScope();
            ICommandRunner<CacheDestroyCommandOptions> runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<CacheDestroyCommandOptions>>();
            return runner.Run(options);
        }
    }

}
