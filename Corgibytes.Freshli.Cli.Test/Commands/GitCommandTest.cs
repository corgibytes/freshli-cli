using Corgibytes.Freshli.Cli.Commands.Git;
using FluentAssertions;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Commands;

public class GitCommandTest
{
    [Fact]
    public void Verify_git_checkout_history_configuration()
    {
        CheckoutHistoryCommand checkoutHistoryCommand = new();
        checkoutHistoryCommand.Handler.Should().NotBeNull();
    }

    [Fact]
    public void Verify_git_clone_configuration()
    {
        GitCloneCommand gitCloneCommand = new();
        gitCloneCommand.Handler.Should().NotBeNull();
    }
}

