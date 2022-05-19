using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class CacheCommand : Command
    {
        public CacheCommand()
            : base("cache", "Manages the local cache database")
        {
            CachePrepareCommand prepare = new();
            AddCommand(prepare);

            CacheDestroyCommand destroy = new();
            AddCommand(destroy);
        }
    }

    public class CachePrepareCommand : RunnableCommand<CachePrepareCommandOptions>
    {
        public CachePrepareCommand()
            : base("prepare", "Ensures the cache directory exists and contains a valid cache database.")
        {}
    }

    public class CacheDestroyCommand : RunnableCommand<CacheDestroyCommandOptions>
    {
        public CacheDestroyCommand()
            : base("destroy", "Deletes the Freshli cache.")
        {
            Option<bool> forceOption = new("--force", "Don't prompt to confirm destruction of cache.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };

            AddOption(forceOption);
        }
    }

}
