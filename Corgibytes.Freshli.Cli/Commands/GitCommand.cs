using System.CommandLine;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands.Git;

namespace Corgibytes.Freshli.Cli.Commands;

public class GitCommand : Command
{
    public GitCommand() : base("git", "Uses git to traverse through a repository's history")
    {
        CheckoutHistoryCommand checkoutHistoryCommand = new();
        AddCommand(checkoutHistoryCommand);
    }
}

