using System.CommandLine;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Commands.Git;

public class ComputeHistoryCommandTest : FreshliTest
{
    public ComputeHistoryCommandTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [MethodData(nameof(DataForVerifyOptionConfiguration))]
    public void Verify_option_configuration(string alias, ArgumentArity arity, int elementIndex)
    {
        var computeHistoryCommand = new ComputeHistoryCommand();
        var option = computeHistoryCommand.Options.ElementAt(elementIndex);

        option.Name.Should().Be(alias);
        option.Arity.Should().BeEquivalentTo(arity);
    }

    [Fact]
    public void Verify_argument_configuration()
    {
        var computeHistoryCommand = new ComputeHistoryCommand();
        var argument = computeHistoryCommand.Arguments.ElementAt(0);

        argument.Name.Should().Be("repository-id");
        argument.Arity.Should().BeEquivalentTo(ArgumentArity.ExactlyOne);
    }

    private static TheoryData<string, ArgumentArity, int> DataForVerifyOptionConfiguration() =>
        new()
        {
            { "history-interval", ArgumentArity.ZeroOrOne, 0 },
            { "git-path", ArgumentArity.ZeroOrOne, 1 }
        };
}
