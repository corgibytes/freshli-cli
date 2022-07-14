using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandOptions.Git;

namespace Corgibytes.Freshli.Cli.Commands.Git;

public class GitCloneCommand : RunnableCommand<GitCloneCommand, GitCloneCommandOptions>
{
    public GitCloneCommand()
        : base("clone", "Clone a repository for Freshli to examine")
    {
        Argument<string> repoUrlArgument = new("repo-url", "The URL to the repository to clone")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        AddArgument(repoUrlArgument);

        Option<string> gitPathOption = new("--git-path",
            description: "The path to the Git executable to use",
            getDefaultValue: () => "git")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };

        AddOption(gitPathOption);

        Option<string> branch = new("--branch",
            "The branch to check out on the repository")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };

        AddOption(branch);
    }
}
