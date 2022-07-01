using System.CommandLine;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Commands.Git;

public class CheckoutHistoryCommandTest : FreshliTest
{
    public CheckoutHistoryCommandTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public void Verify_git_path_option_configuration()
    {
        CheckoutHistoryCommand checkoutHistoryCommand = new();
        var option = checkoutHistoryCommand.Options.ElementAt(0);

        option.Name.Should().Be("git-path");
        option.Arity.Should().BeEquivalentTo(ArgumentArity.ZeroOrOne);
    }

    [Theory]
    [MethodData(nameof(DataForVerifyArgumentConfiguration))]
    public void Verify_argument_configuration(string alias, ArgumentArity arity, int elementIndex)
    {
        CheckoutHistoryCommand checkoutHistoryCommand = new();
        var argument = checkoutHistoryCommand.Arguments.ElementAt(elementIndex);

        argument.Name.Should().Be(alias);
        argument.Arity.Should().BeEquivalentTo(arity);
    }

    private static TheoryData<string, ArgumentArity, int> DataForVerifyArgumentConfiguration()
    {
        return new()
        {
            {"repository-id", ArgumentArity.ExactlyOne, 0},
            {"sha", ArgumentArity.ExactlyOne, 1}
        };
    }
}

