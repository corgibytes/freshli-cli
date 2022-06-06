using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Cache;

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



}
