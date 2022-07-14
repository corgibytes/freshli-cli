using System.CommandLine;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands.Git;

public class CheckoutHistoryCommand : RunnableCommand<CheckoutHistoryCommand, CheckoutHistoryCommandOptions>
{
    public CheckoutHistoryCommand() : base("checkout-history",
        "Used to checkout a specific historical point for a given repository.")
    {
        Argument<string> repositoryId = new("repository-id", "Id of the repository")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        Argument<string> sha = new("sha", "Git commit sha identifier") { Arity = ArgumentArity.ExactlyOne };

        Option<FileInfo> gitPath = new("--git-path", "Path to the git binary. Default = 'git'")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        AddArgument(repositoryId);
        AddArgument(sha);
        AddOption(gitPath);
    }
}
