using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class CachePrepareCommand : RunnableCommand<CacheCommand, CachePrepareCommandOptions>
{
    public CachePrepareCommand()
        : base("prepare", CliOutput.Help_CachePrepareCommand_Description)
    {
    }
}
