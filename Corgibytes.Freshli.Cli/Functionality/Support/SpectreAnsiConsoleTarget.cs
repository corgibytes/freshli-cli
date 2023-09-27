using System.Collections.Generic;
using NLog;
using NLog.Targets;
using Spectre.Console;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public class SpectreAnsiConsoleTarget : TargetWithLayout
{
    private readonly IAnsiConsole _console;

    public SpectreAnsiConsoleTarget(IAnsiConsole console)
    {
        _console = console;
    }

    public SpectreAnsiConsoleTarget(string name, IAnsiConsole console) : this(console)
    {
        Name = name;
    }

    private readonly Dictionary<LogLevel, string> _logLevelColors = new()
    {
        { LogLevel.Debug, "grey" },
        { LogLevel.Info, "white" },
        { LogLevel.Error, "red" },
        { LogLevel.Fatal, "bold red" },
        { LogLevel.Warn, "yellow" },
        { LogLevel.Trace, "dim grey" }
    };

    protected override void Write(LogEventInfo logEventInfo)
    {
        var message = RenderLogEvent(Layout, logEventInfo);
        var colorizedMessage = $"[{_logLevelColors[logEventInfo.Level]}]{message.EscapeMarkup()}[/]";

        _console.MarkupLine(colorizedMessage);
    }
}
