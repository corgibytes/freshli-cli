using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands;

public class GitCommand : Command
{
    public GitCommand()
        : base("git", CliOutput.Help_GitCommand_Description)
    {
        GitCloneCommand clone = new();
        AddCommand(clone);

        CheckoutHistoryCommand checkoutHistoryCommand = new();
        AddCommand(checkoutHistoryCommand);

        ComputeHistoryCommand computeHistoryCommand = new();
        AddCommand(computeHistoryCommand);
    }
}
