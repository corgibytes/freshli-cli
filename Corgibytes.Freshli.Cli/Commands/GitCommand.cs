using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands;

public class GitCommand : Command
{
    public GitCommand()
        : base("git", CliOutput.Help_GitCommand_Description)
    {
        var clone = new GitCloneCommand();
        AddCommand(clone);

        var checkoutHistoryCommand = new CheckoutHistoryCommand();
        AddCommand(checkoutHistoryCommand);

        var computeHistoryCommand = new ComputeHistoryCommand();
        AddCommand(computeHistoryCommand);
    }
}
