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
    [InlineData("--cache-dir")]
    public void Verify_cache_dir_option_configuration(string alias)
    {
        var configuration = new Mock<IConfiguration>();
        var mainCommand = new MainCommand(configuration.Object);
        mainCommand.VerifyAlias(alias, ArgumentArity.ExactlyOne, false);
    }
}
