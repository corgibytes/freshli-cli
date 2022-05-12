using System;
using System.CommandLine;
using System.IO;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class MainCommand : RootCommand
    {
        public MainCommand() : base("Root Command")
        {
            Option<DirectoryInfo> cacheDirOption = new(
                new[] {"--cache-dir"},
                description: "The location for storing temporary files",
                getDefaultValue: () => new DirectoryInfo(Environment.GetEnvironmentVariable("HOME") + "/.freshli"))
            {
                Arity = ArgumentArity.ExactlyOne
            };

            AddGlobalOption(cacheDirOption);
        }
    }
}
