using System.CommandLine;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class MainCommand : RootCommand
    {
        public MainCommand() : base("Root Command")
        {
            Option<DirectoryInfo> cacheDirOption = new(
                new[] {"--cache-dir"},
                description: "The location for storing temporary files",
                getDefaultValue: () => CacheContext.CacheDirDefault)
            {
                Arity = ArgumentArity.ExactlyOne
            };

            AddGlobalOption(cacheDirOption);
        }
    }
}
