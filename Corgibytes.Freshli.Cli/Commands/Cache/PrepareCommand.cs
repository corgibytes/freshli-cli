using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class CachePrepareCommand : RunnableCommand<CachePrepareCommandOptions>
{
    public CachePrepareCommand()
        : base("prepare", "Ensures the cache directory exists and contains a valid cache database.")
    { }
}
