using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands;

public class CacheCommand : Command
{
    public CacheCommand() : base("cache", CliOutput.Help_CacheCommand_Description)
    {
        CachePrepareCommand prepare = new();
        AddCommand(prepare);

        CacheDestroyCommand destroy = new();
        AddCommand(destroy);
    }
}
