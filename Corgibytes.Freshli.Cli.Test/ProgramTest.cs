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
    private const string ApplicationShutdownLogMessageForConsole =
        "INFO | Microsoft.Hosting.Lifetime:0 | Application is shutting down...";

    private const string HostShutdownLogMessageForConsole =
        "DEBUG | Microsoft.Extensions.Hosting.Internal.Host:0 | Hosting stopping";

    private const string ApplicationShutdownLogMessageForFile =
        "INFO | Microsoft.Hosting.Lifetime:0 | Application is shutting down...";

#pragma warning disable SYSLIB1045
    private readonly Regex _trailingWhitespaceRegex = new(@"\s+$", RegexOptions.Multiline);
    private readonly Regex _logMessagesContainingNewlinesRegex =
        new(@"(\r?\n)([^\r\n\d])", RegexOptions.Multiline);
#pragma warning restore SYSLIB1045

    public ProgramTest()
    {
        Console.SetOut(_consoleOutput);

        MainCommand.ShouldIncludeLoadServiceCommand = true;
        MainCommand.ShouldIncludeFailCommand = true;
    }

    private string CleanupLogOutput(string logMessages)
    {
        var result = _trailingWhitespaceRegex.Replace(logMessages, "");
        result = _logMessagesContainingNewlinesRegex.Replace(result, " $2");
        return result;
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Validate_Main_loglevel_debug()
    {
        await Task.Run(async () => await Program.Main("--loglevel", "Debug"));

        var cleanedOutput = CleanupLogOutput(_consoleOutput.ToString().StripAnsi());

        cleanedOutput.Should().Contain(HostShutdownLogMessageForConsole);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Validate_Main_loglevel_info()
    {
        await Task.Run(async () => await Program.Main("--loglevel", "Info", "load-service"));

        var cleanedOutput = CleanupLogOutput(_consoleOutput.ToString().StripAnsi());

        cleanedOutput.Should().NotContain(HostShutdownLogMessageForConsole);
        cleanedOutput.Should().Contain(ApplicationShutdownLogMessageForConsole);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Validate_Main_loglevel_default()
    {
        await Task.Run(async () => await Program.Main());

        var cleanedOutput = CleanupLogOutput(_consoleOutput.ToString().StripAnsi());

        cleanedOutput.Should().NotContain(HostShutdownLogMessageForConsole);
        cleanedOutput.Should().NotContain(ApplicationShutdownLogMessageForConsole);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Validate_Main_logfile()
    {
        var testfile = "testlog.log";

        await Task.Run(async () => await Program.Main("--loglevel", "Info", "--logfile", testfile));

        // force NLog to close the log file so that we can open it to read from
        var target = LogManager.Configuration.FindTargetByName("file") as FileTarget;
        target!.KeepFileOpen = false;

        await using var file = File.Open(testfile, FileMode.Open, FileAccess.Read);
        var logFileContent = await file.ReadToEndAsync();

        var cleanedOutput = CleanupLogOutput(_consoleOutput.ToString().StripAnsi());
        cleanedOutput.Should().NotContain(ApplicationShutdownLogMessageForConsole);
        logFileContent.Should().Contain(ApplicationShutdownLogMessageForFile);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task ValidateExceptionsInActivityHandlersWriteToLog()
    {
        await Task.Run(async () => await Program.Main("fail"));

#pragma warning disable SYSLIB1045
        _consoleOutput.ToString().StripAnsi().Should().MatchRegex(new Regex(
            "ERROR | .*System.Exception: Simulating failure from an activity$", RegexOptions.Multiline
        ));
#pragma warning restore SYSLIB1045
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task ValidateServiceProviderIsLoaded()
    {
        MainCommand.ShouldIncludeLoadServiceCommand = true;
        await Task.Run(async () => await Program.Main("load-service"));

        var cleanedOutput = CleanupLogOutput(_consoleOutput.ToString().StripAnsi());
        cleanedOutput.Should().Contain("All good! Service provider is not null.");
        cleanedOutput.Should()
            .NotContain("Simulating loading the service provider, but the provider is null.");
    }
}
