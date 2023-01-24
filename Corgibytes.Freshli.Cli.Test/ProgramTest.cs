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

static class StringExtensions
{
    public static string StripAnsi(this string value)
    {
        return Regex.Replace (value, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
    }
}

[IntegrationTest]
public class ProgramTest
{
    private readonly StringWriter _consoleOutput = new();
    private readonly string _applicationShutdownLogMessageForConsole =
        $"INFO | Microsoft.Hosting.Lifetime:0 | Application is {Environment.NewLine}shutting down...";

    private readonly string _hostShutdownLogMessageForConsole =
        $"DEBUG | Microsoft.Extensions.Hosting.Internal.Host:0 |{Environment.NewLine}Hosting stopping";

    private readonly string _applicationShutdownLogMessageForFile =
        "INFO | Microsoft.Hosting.Lifetime:0 | Application is shutting down...";

    public ProgramTest()
    {
        Console.SetOut(_consoleOutput);

        MainCommand.ShouldIncludeLoadServiceCommand = true;
        MainCommand.ShouldIncludeFailCommand = true;
    }

    [Fact(Timeout = 500)]
    public async Task Validate_Main_loglevel_debug()
    {
        await Task.Run(async () => await Program.Main("--loglevel", "Debug"));

        var cleanedOutput = _consoleOutput.ToString().StripAnsi();

        cleanedOutput.Should().Contain(_hostShutdownLogMessageForConsole);
    }

    [Fact(Timeout = 500)]
    public async Task Validate_Main_loglevel_info()
    {
        await Task.Run(async () => await Program.Main("--loglevel", "Info", "load-service"));

        var cleanedOutput = _consoleOutput.ToString().StripAnsi();

        cleanedOutput.Should().NotContain(_hostShutdownLogMessageForConsole);
        cleanedOutput.Should().Contain(_applicationShutdownLogMessageForConsole);
    }

    [Fact(Timeout = 500)]
    public async Task Validate_Main_loglevel_default()
    {
        await Task.Run(async () => await Program.Main());

        var cleanedOutput = _consoleOutput.ToString().StripAnsi();

        cleanedOutput.Should().NotContain(_hostShutdownLogMessageForConsole);
        cleanedOutput.Should().NotContain(_applicationShutdownLogMessageForConsole);
    }

    [Fact(Timeout = 500)]
    public async Task Validate_Main_logfile()
    {
        var testfile = "testlog.log";

        await Task.Run(async () => await Program.Main("--loglevel", "Info", "--logfile", testfile));

        // force NLog to close the log file so that we can open it to read from
        var target = LogManager.Configuration.FindTargetByName("file") as FileTarget;
        target!.KeepFileOpen = false;

        await using var file = File.Open(testfile, FileMode.Open, FileAccess.Read);
        var logFileContent = file.ReadToEnd();
        _consoleOutput.ToString().StripAnsi().Should().NotContain(_applicationShutdownLogMessageForConsole);
        logFileContent.Should().Contain(_applicationShutdownLogMessageForFile);
    }

    [Fact(Timeout = 500)]
    public async Task ValidateExceptionsInActivityHandlersWriteToLog()
    {
        await Task.Run(async () => await Program.Main("fail"));

#pragma warning disable SYSLIB1045
        _consoleOutput.ToString().StripAnsi().Should().MatchRegex(new Regex(
            "ERROR | .*System.Exception: Simulating failure from an activity$", RegexOptions.Multiline
        ));
#pragma warning restore SYSLIB1045
    }

    [Fact(Timeout = 500)]
    public async Task ValidateServiceProviderIsLoaded()
    {
        MainCommand.ShouldIncludeLoadServiceCommand = true;
        await Task.Run(async () => await Program.Main("load-service"));

        var cleanedOutput = _consoleOutput.ToString().StripAnsi();
        cleanedOutput.Should().Contain("All good! Service provider is not null.");
        cleanedOutput.Should()
            .NotContain("Simulating loading the service provider, but the provider is null.");
    }
}
