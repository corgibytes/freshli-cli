using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Commands;

public class ScanCommandTest : FreshliTest
{
    public ScanCommandTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Verify_path_argument_configuration()
    {
        ScanCommand scanCommand = new();

        scanCommand.Arguments.Should().HaveCount(1);

        var arg = scanCommand.Arguments.ElementAt(0);

        arg.Name.Should().Be("path");
        arg.Arity.Should().BeEquivalentTo(ArgumentArity.ExactlyOne);
    }

    [Theory]
    [MethodData(nameof(DataForVerifyOptionConfigurations))]
    public void VerifyOptionConfigurations(string alias, ArgumentArity arity, bool allowsMultiples) =>
        TestHelpers.VerifyAlias<ScanCommand>(alias, arity, allowsMultiples);

    private static TheoryData<string, ArgumentArity, bool> DataForVerifyOptionConfigurations() =>
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
        ScanCommand scanCommand = new();
        scanCommand.Handler.Should().NotBeNull();
    }
}
