using System.Collections.Generic;
using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Commands;

[UnitTest]
public class MainCommandTest : FreshliTest
{
    public MainCommandTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Verify_no_handler_configuration()
    {
        var configuration = new Mock<IConfiguration>();
        var mainCommand = new MainCommand(configuration.Object);
        mainCommand.Handler.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(OptionsData))]
    public void VerifyOptions(string alias, ArgumentArity arity)
    {
        var configuration = new Mock<IConfiguration>();
        var mainCommand = new MainCommand(configuration.Object);
        mainCommand.VerifyAlias(alias, arity, false);
    }

    public static IEnumerable<object?[]> OptionsData() =>
        new List<object?[]>
        {
            new object?[] { "--workers", ArgumentArity.ExactlyOne },
            new object?[] { "--cache-dir", ArgumentArity.ExactlyOne }
        };
}
