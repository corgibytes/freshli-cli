using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using FluentAssertions;
using NLog;
using NLog.Targets;
using ServiceStack;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test;

[IntegrationTest]
public class ProgramTest
{
    private readonly StringWriter _consoleOutput = new();

    public ProgramTest() => Console.SetOut(_consoleOutput);

    [Fact]
    public void Validate_Main_loglevel_debug()
    {
        var task = Task.Run(async () => await Program.Main("--loglevel", "Debug"));
        task.Wait();

        _consoleOutput.ToString().Should()
            .Contain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
    }

    [Fact]
    public void Validate_Main_loglevel_info()
    {
        var task = Task.Run(async () => await Program.Main("--loglevel", "Info"));
        task.Wait();

        _consoleOutput.ToString().Should()
            .NotContain("DEBUG|Microsoft.Extensions.Hosting.Internal.Host:0|Hosting stopping");
        _consoleOutput.ToString().Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void Validate_Main_loglevel_default()
    {
        var task = Task.Run(async () => await Program.Main());
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

        var task = Task.Run(async () => await Program.Main("--loglevel", "Info", "--logfile", testfile));
        task.Wait();

        // force NLog to close the log file so that we can open it to read from
        var target = LogManager.Configuration.FindTargetByName("file") as FileTarget;
        target!.KeepFileOpen = false;

        using var file = File.Open(testfile, FileMode.Open, FileAccess.Read);
        var logFileContent = file.ReadToEnd();
        _consoleOutput.ToString().Should()
            .NotContain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
        logFileContent.Should().Contain("INFO|Microsoft.Hosting.Lifetime:0|Application is shutting down...");
    }

    [Fact]
    public void ValidateExceptionsInActivityHandlersWriteToLog()
    {
        MainCommand.ShouldIncludeFailCommand = true;

        var task = Task.Run(async () => await Program.Main("fail"));
        task.Wait();

        _consoleOutput.ToString().Should().MatchRegex(new Regex(
            "ERROR|.*System.Exception: Simulating failure from an activity$", RegexOptions.Multiline
        ));
    }

    [Fact]
    public void ValidateServiceProviderIsLoaded()
    {
        MainCommand.ShouldIncludeLoadServiceCommand = true;
        var task = Task.Run(async () => await Program.Main("load-service"));
        task.Wait();

        _consoleOutput.ToString().Should().Contain("All good! Service provider is not null.");
        _consoleOutput.ToString().Should()
            .NotContain("Simulating loading the service provider, but the provider is null.");
    }

    private static Regex SimulatedFailureMessageRegex() =>
#pragma warning disable SYSLIB1045
        new("^ERROR|.*System.Exception: Simulating failure from an activity$", RegexOptions.Multiline);
#pragma warning restore SYSLIB1045

}
