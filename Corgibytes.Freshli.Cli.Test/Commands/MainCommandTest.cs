using System.CommandLine;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Commands
{
    public class MainCommandTest : FreshliTest
    {
        public MainCommandTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Verify_no_handler_configuration()
        {
            MainCommand mainCommand = new();
            mainCommand.Handler.Should().BeNull();
        }

        [Theory]
        [InlineData("--cache-dir")]
        public void Verify_cache_dir_option_configuration(string alias)
        {
            TestHelpers.VerifyAlias<MainCommand>(alias, ArgumentArity.ExactlyOne, false);
        }
    }
}
