using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test;

public class ProgramTest : FreshliTest
{
    public ProgramTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Validate_Main_loglevel_debug()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        var task = Task.Run(() => Program.Main("--loglevel", "Debug"));
        task.Wait();

        stringWriter.ToString().Should().Contain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
    }

    [Fact]
    public void Validate_Main_loglevel_info()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        var task = Task.Run(() => Program.Main("--loglevel", "Info"));
        task.Wait();

        stringWriter.ToString().Should()
            .NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
        stringWriter.ToString().Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void Validate_Main_loglevel_default()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        var task = Task.Run(() => Program.Main());
        task.Wait();

        stringWriter.ToString().Should()
            .NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
        stringWriter.ToString().Should()
            .NotContain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void Validate_Main_logfile()
    {
        Console.Out.Write(Environment.CurrentDirectory);
        var testfile = "testlog.log";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        var task = Task.Run(() => Program.Main("--loglevel", "Info", "--logfile", testfile));
        task.Wait();

        var logFileContent = File.ReadAllText(testfile);
        stringWriter.ToString().Should()
            .NotContain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
        logFileContent.Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }
}
