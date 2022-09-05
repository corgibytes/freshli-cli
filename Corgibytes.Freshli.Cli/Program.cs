using System;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.IoC;
using Corgibytes.Freshli.Cli.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using NLog.Targets;
using static System.String;
using LogLevel = NLog.LogLevel;

namespace Corgibytes.Freshli.Cli;

public static class Program
{
    private const string DefaultLogLayout =
        "${date}|${level:uppercase=true:padding=5}|${logger}:${callsite-linenumber}|${message} ${exception}";

    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public static async Task<int> Main(params string[] args)
    {
        var cmdBuilder = CreateCommandLineBuilder();
        return await cmdBuilder.UseDefaults()
            .Build()
            .InvokeAsync(args);
    }

    private static void ConfigureLogging(string? consoleLogLevel, string? logfile)
    {
        var desiredLevel = LogLevel.FromString(consoleLogLevel);

        var config = new LoggingConfiguration();
        if (IsNullOrEmpty(logfile))
        {
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = DefaultLogLayout;
            config.LoggingRules.Add(new LoggingRule("*", desiredLevel, consoleTarget));
        }
        else
        {
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = logfile;
            fileTarget.Layout = DefaultLogLayout;
            config.LoggingRules.Add(new LoggingRule("*", desiredLevel, fileTarget));
        }

        LogManager.Configuration = config;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddNLog();
            })
            .UseNLog()
            .ConfigureServices((_, services) => { new FreshliServiceBuilder(services).Register(); });


    public static CommandLineBuilder CreateCommandLineBuilder()
    {
        var builder = new CommandLineBuilder(new MainCommand())
            .UseHost(CreateHostBuilder)
            .AddMiddleware(async (context, next) =>
            {
                await Task.Run(() => ConfigureLogging(
                    context.ParseResult.GetOptionValueByName<string>("loglevel"),
                    context.ParseResult.GetOptionValueByName<string>("logfile"))
                );

                await next(context);
            })
            .AddMiddleware(async (context, next) => { await LogExecution(context, next); })
            .UseExceptionHandler()
            .UseHelp();

        return builder;
    }

    private static async Task LogExecution(InvocationContext context, Func<InvocationContext, Task> next)
    {
        var commandLine = context.ParseResult.ToString();

        try
        {
            var callingMessage = $"[Command Execution Invocation Started  - {commandLine} ]\n";
            var doneMessage = $"[Command Execution Invocation Ended - {commandLine} ]\n";

            context.Console.Out.Write(callingMessage);
            s_logger.Trace(callingMessage);

            await next(context);

            context.Console.Out.Write(doneMessage);
            s_logger.Trace(doneMessage);
        }
        catch (Exception e)
        {
            var message = $"[Unhandled Exception - {commandLine}] - {e.Message}";
            context.Console.Out.WriteLine(
                $"{message} - Take a look at the log for detailed information.\n. {e.StackTrace}");

            LogException(context.Console.Out, s_logger, e);
            s_logger.Error($"{message} - {e.StackTrace}");
        }
    }

    private static void LogException(IStandardStreamWriter output, Logger logger, Exception error)
    {
        logger.Error(error.Message);
        output.WriteLine(error.Message);
        if (error.StackTrace != null)
        {
            logger.Error(error.StackTrace);
            output.WriteLine(error.StackTrace);
        }

        if (error.InnerException == null)
        {
            return;
        }

        logger.Error("==Inner Exception==");
        output.WriteLine("==Inner Exception==");
        LogException(output, logger, error.InnerException);
        logger.Error("==End Inner Exception==");
        output.WriteLine("==End Inner Exception==");
    }
}
