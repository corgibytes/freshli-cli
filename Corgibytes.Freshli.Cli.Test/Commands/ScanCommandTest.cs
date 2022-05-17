using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Commands
{
    public class ScanCommandTest : FreshliTest
    {
        private readonly TestConsole _console = new();

        public ScanCommandTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Verify_path_argument_configuration()
        {
            ScanCommand scanCommand = new();

            scanCommand.Arguments.Should().HaveCount(1);

            Argument arg = scanCommand.Arguments.ElementAt(0);

            arg.Name.Should().Be("path");
            arg.Arity.Should().BeEquivalentTo(ArgumentArity.ExactlyOne);
        }

        [Theory]
        [InlineData("--format")]
        [InlineData("-f")]
        public void Verify_format_option_configuration(string alias)
        {
            TestHelpers.VerifyAlias<ScanCommand>(alias, ArgumentArity.ExactlyOne, false);
        }

        [Theory]
        [InlineData("--output")]
        [InlineData("-o")]
        public void Verify_output_options_configuration(string alias)
        {
            TestHelpers.VerifyAlias<ScanCommand>(alias, ArgumentArity.OneOrMore, true);
        }

        [Fact]
        public void Verify_handler_configuration()
        {
            ScanCommand scanCommand = new();
            scanCommand.Handler.Should().NotBeNull();
        }

        [Fact(Skip = "Will until we have a way to mock the freshli lib call")]
        public async Task  Verify_handler_is_executed()
        {
            CommandLineBuilder cmdBuilder = Program.CreateCommandLineBuilder();
            await cmdBuilder.UseDefaults()
                .Build().InvokeAsync("scan http://github.com/corgibytes/freshli-ruby.git  -f yaml", _console);

            _console.Out.ToString().Should().Contain("[ scan <.> [ -f <Csv> ]");
            _console.Out.ToString().Should().Contain("Command Execution Invocation Started");
            _console.Out.ToString().Should().Contain("Command Execution Invocation Ended");
            _console.Out.ToString().Should().NotContain("Exception has been thrown by the target of an invocation");
        }
    }
}
