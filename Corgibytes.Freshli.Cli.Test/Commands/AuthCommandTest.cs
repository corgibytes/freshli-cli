using System;
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
    public class AuthCommandTest: FreshliTest
    {
        private readonly TestConsole console = new();

        public AuthCommandTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Verify_handler_configuration()
        {
            AuthCommand scanCommand = new();
            scanCommand.Handler.Should().NotBeNull();
        }

        [Fact]
        public async Task  Verify_handler_is_executed()
        {
            CommandLineBuilder cmdBuilder = Program.CreateCommandLineBuilder();
            await cmdBuilder.UseDefaults()
                .Build().InvokeAsync("auth", console);
                
            console.Out.ToString().Should().Contain("[ auth *[ --format <Json> ] *[ --output <Console> ] ] ]]");
            console.Out.ToString().Should().Contain("Command Execution Invocation Started");
            console.Out.ToString().Should().Contain("Executing auth command handler");
            console.Out.ToString().Should().Contain("Command Execution Invocation Ended");
            console.Out.ToString().Should().Contain("Exception has been thrown by the target of an invocation");
        }
    }
}
