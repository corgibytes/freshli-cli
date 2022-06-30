using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Git;

namespace Corgibytes.Freshli.Cli.Commands;

public class GitCommand : Command
{
    public GitCommand()
        : base("git", "Uses git to traverse through a repository's history")
    {
        GitCloneCommand clone = new();
        AddCommand(clone);

        CheckoutHistoryCommand checkoutHistoryCommand = new();
        AddCommand(checkoutHistoryCommand);
    }
}
