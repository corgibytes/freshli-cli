using Corgibytes.Freshli.Cli.Commands.Git;
using FluentAssertions;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Commands;

[UnitTest]
public class GitCommandTest
{
    [Fact]
    public void Verify_git_checkout_history_configuration()
    {
        var checkoutHistoryCommand = new CheckoutHistoryCommand();
        checkoutHistoryCommand.Handler.Should().NotBeNull();
    }

    [Fact]
    public void Verify_git_clone_configuration()
    {
        var gitCloneCommand = new GitCloneCommand();
        gitCloneCommand.Handler.Should().NotBeNull();
    }
}
