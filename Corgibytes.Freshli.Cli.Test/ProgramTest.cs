using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test;

public class ProgramTest : FreshliTest
{
    private readonly StringWriter _consoleOutput = new();

    public ProgramTest(ITestOutputHelper output) : base(output)
    {
        Console.SetOut(_consoleOutput);
    }

    [Fact]
    public void Validate_Main_loglevel_debug()
    {
        var task = Task.Run(() => Program.Main("--loglevel", "Debug"));
        task.Wait();

        _consoleOutput.ToString().Should().Contain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
    }

    [Fact]
    public void Validate_Main_loglevel_info()
    {
        var task = Task.Run(() => Program.Main("--loglevel", "Info"));
        task.Wait();

        _consoleOutput.ToString().Should()
            .NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
        _consoleOutput.ToString().Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void Validate_Main_loglevel_default()
    {
        var task = Task.Run(() => Program.Main());
        task.Wait();

        _consoleOutput.ToString().Should()
            .NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
        _consoleOutput.ToString().Should()
            .NotContain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void Validate_Main_logfile()
    {
        var testfile = "testlog.log";

        var task = Task.Run(() => Program.Main("--loglevel", "Info", "--logfile", testfile));
        task.Wait();

        var logFileContent = File.ReadAllText(testfile);
        _consoleOutput.ToString().Should()
            .NotContain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
        logFileContent.Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void ValidateExceptionsInActivityHandlersWriteToLog()
    {
        MainCommand.ShouldIncludeFailCommand = true;

        var task = Task.Run(() => Program.Main("fail"));
        task.Wait();

        _consoleOutput.ToString().Should().MatchRegex(new Regex(
            "^ERROR|.*System.Exception: Simulating failure from an activity$", RegexOptions.Multiline
        ));
    }
}
