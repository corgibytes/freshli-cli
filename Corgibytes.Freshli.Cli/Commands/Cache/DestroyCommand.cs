using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class CacheDestroyCommand : RunnableCommand<CacheCommand, CacheDestroyCommandOptions>
{
    public CacheDestroyCommand()
        : base("destroy", CliOutput.Help_CacheDestroyCommand_Description)
    {
        var forceOption = new Option<bool>("--force", CliOutput.Help_CacheDestoyCommand_Option_Force)
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        AddOption(forceOption);
    }
}
