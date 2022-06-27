using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class CacheDestroyCommand : RunnableCommand<CacheDestroyCommandOptions>
{
    public CacheDestroyCommand()
        : base("destroy", CliOutput.Help_CacheDestroyCommand_Description)
    {
        Option<bool> forceOption = new("--force", CliOutput.Help_CacheDestoyCommand_Option_Force)
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        AddOption(forceOption);
    }
}
