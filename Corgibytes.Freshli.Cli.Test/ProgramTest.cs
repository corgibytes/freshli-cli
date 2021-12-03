using System;
using System.IO;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test
{
    public class ProgramTest : FreshliTest
    {
        public ProgramTest(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void Validate_Main_loglevel_debug()
        {
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            Program.Main(new string [] {"--loglevel", "Debug" });
            stringWriter.ToString().Should().Contain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
        }

        [Fact]
        public void Validate_Main_loglevel_info()
        {
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            Program.Main(new string [] {"--loglevel", "Info" });
            stringWriter.ToString().Should().NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
            stringWriter.ToString().Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
        }

        [Fact]
        public void Validate_Main_loglevel_default()
        {
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            Program.Main(null);
            stringWriter.ToString().Should().NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
            stringWriter.ToString().Should().NotContain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
        }

    }
}
