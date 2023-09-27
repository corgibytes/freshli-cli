using System.CommandLine;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands.Scan;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Commands.Scan;

[UnitTest]
public class ScanCommandTest : FreshliTest
{
    public ScanCommandTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Verify_path_argument_configuration()
    {
        var scanCommand = new ScanCommand();

        scanCommand.Arguments.Should().HaveCount(1);

        var arg = scanCommand.Arguments.ElementAt(0);

        arg.Name.Should().Be("path");
        arg.Arity.Should().BeEquivalentTo(ArgumentArity.ExactlyOne);
    }

    [Theory]
    [MemberData(nameof(DataForVerifyOptionConfigurations))]
    public void VerifyOptionConfigurations(string alias, ArgumentArity arity, bool allowsMultiples) =>
        TestHelpers.VerifyAlias<ScanCommand>(alias, arity, allowsMultiples);

    public static TheoryData<string, ArgumentArity, bool> DataForVerifyOptionConfigurations() =>
        new()
        {
            { "--format", ArgumentArity.ExactlyOne, false },
            { "-f", ArgumentArity.ExactlyOne, false },
            { "--output", ArgumentArity.OneOrMore, true },
            { "-o", ArgumentArity.OneOrMore, true }
        };

    [Fact]
    public void Verify_handler_configuration()
    {
        var scanCommand = new ScanCommand();
        scanCommand.Handler.Should().NotBeNull();
    }
}
