using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions.Git;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands.Git;

public class GitCloneCommand : RunnableCommand<GitCloneCommand, GitCloneCommandOptions>
{
    public GitCloneCommand()
        : base("clone", CliOutput.Help_GitCloneCommand_Description)
    {
        Argument<string> repoUrlArgument = new("repo-url", CliOutput.Help_GitCloneCommand_Argument_Repo_Url)
        {
            Arity = ArgumentArity.ExactlyOne
        };

        AddArgument(repoUrlArgument);

        Option<string> gitPathOption = new("--git-path",
            description: CliOutput.Help_GitCloneCommand_Option_Git_Path,
            getDefaultValue: () => "git")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };

        AddOption(gitPathOption);

        Option<string> branch = new("--branch",
            CliOutput.Help_GitCloneCommand_Option_Branch)
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };

        AddOption(branch);
    }
}
