using System.CommandLine;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands;

public class MainCommand : RootCommand
{
    public MainCommand() : base(CliOutput.Help_MainCommand_Description)
    {
        Option<string> cacheDirOption = new(
            new[] { "--cache-dir" },
            description: CliOutput.Help_Option_CacheDir_Description,
            getDefaultValue: () => CacheContext.DefaultCacheDir)
        { Arity = ArgumentArity.ExactlyOne };
        AddGlobalOption(cacheDirOption);
    }
}
