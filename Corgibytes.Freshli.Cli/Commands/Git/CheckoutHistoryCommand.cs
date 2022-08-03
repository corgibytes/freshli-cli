using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions.Git;

namespace Corgibytes.Freshli.Cli.Commands.Git;

public class CheckoutHistoryCommand : RunnableCommand<CheckoutHistoryCommand, CheckoutHistoryCommandOptions>
{
    public CheckoutHistoryCommand() : base("checkout-history",
        "Used to checkout a specific historical point for a given repository.")
    {
        var repositoryId = new Argument<string>("repository-id", "Id of the repository")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        var sha =
            new Argument<string>("sha", "Git commit sha identifier") { Arity = ArgumentArity.ExactlyOne };

        var gitPath = new Option<string>("--git-path",
            description: "Path to the git binary. Default = 'git'", getDefaultValue: () => "git")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        AddArgument(repositoryId);
        AddArgument(sha);
        AddOption(gitPath);
    }
}
