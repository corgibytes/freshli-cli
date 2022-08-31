using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class AnalyzeCommand : RunnableCommand<AnalyzeCommand, AnalyzeCommandOptions>
{
    public AnalyzeCommand() : base("analyze", "The primary user-facing command. This command will delegate to other freshli activities to accomplish its work. It will manage work queues to enable parallelization.")
    {
        var gitPath = new Option<string>(
                "git-path",
                description: "Path to the git binary. Default = 'git'",
                getDefaultValue: () => "git"
            )
            { Arity = ArgumentArity.ZeroOrOne };
        AddOption(gitPath);
    }
}
