using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands;

public class CacheCommand : Command
{
    public CacheCommand() : base("cache", CliOutput.Help_CacheCommand_Description)
    {
        var prepare = new CachePrepareCommand();
        AddCommand(prepare);

        var destroy = new CacheDestroyCommand();
        AddCommand(destroy);
    }
}
