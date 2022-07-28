using System;
using Hangfire.Logging;
using Microsoft.Extensions.Logging;
using LogLevel = Hangfire.Logging.LogLevel;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// from https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/AspNetCore/AspNetCoreLog.cs#L24
// This class has been modified from the original version to match the conventions of this project
internal class MicrosoftExtensionsLog : ILog
{
    private static readonly Func<object, Exception?, string> s_messageFormatterFunc = MessageFormatter;

    private readonly ILogger _targetLogger;

    public MicrosoftExtensionsLog(ILogger targetLogger) =>
        _targetLogger = targetLogger ?? throw new ArgumentNullException(nameof(targetLogger));

    public bool Log(LogLevel logLevel, Func<string>? messageFunc, Exception? exception = null)
    {
        var targetLogLevel = ToTargetLogLevel(logLevel);

        // When messageFunc is null, Hangfire.Logging
        // just determines is logging enabled.
        if (messageFunc == null)
        {
            return _targetLogger.IsEnabled(targetLogLevel);
        }

        _targetLogger.Log(targetLogLevel, 0, messageFunc(), exception, s_messageFormatterFunc);
        return true;
    }

    private static Microsoft.Extensions.Logging.LogLevel ToTargetLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warn => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.Fatal => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => Microsoft.Extensions.Logging.LogLevel.None,
        };
    }

    private static string MessageFormatter(object? state, Exception? exception)
    {
        if (state == null)
        {
            return string.Empty;
        }

        var value = state.ToString();
        return value ?? string.Empty;
    }
}
