using System;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using NLog.Targets;
using static System.String;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;
using LogLevel = NLog.LogLevel;

namespace Corgibytes.Freshli.Cli;

public class Program
{
    private const string DefaultLogLayout =
        "${date}|${level:uppercase=true:padding=5}|${logger}:${callsite-linenumber}|${message} ${exception}";

    private static ILogger<Program>? Logger { get; set; }
    private static IHostedService? HangfireBackgroundService { get; set; }
    private static BackgroundJobServerOptions? HangfireOptions { get; set; }
    private static IConfiguration Configuration { get; } = new Configuration(new Environment());

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
            .ConfigureServices((_, services) =>
            {
                new FreshliServiceBuilder(services, Configuration).Register();
                Logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
                HangfireBackgroundService = services.BuildServiceProvider().GetRequiredService<IHostedService>();
                HangfireOptions = services.BuildServiceProvider().GetRequiredService<BackgroundJobServerOptions>();
            });


    public static CommandLineBuilder CreateCommandLineBuilder()
    {
        var builder = new CommandLineBuilder(new MainCommand(Configuration))
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
            .AddMiddleware(async (context, next) =>
            {
                var workerCount = context.ParseResult.GetOptionValueByName<int>("workers");
                await Task.Run(() => { HangfireBackgroundService?.StopAsync(CancellationToken.None); });

                if (HangfireOptions != null && workerCount > 0)
                {
                    HangfireOptions.WorkerCount = workerCount;
                }

                HangfireBackgroundService?.StartAsync(new CancellationToken());

                await next(context);
            })
            .UseExceptionHandler()
            .UseHelp();

        return builder;
    }

    private static async Task LogExecution(InvocationContext context, Func<InvocationContext, Task> next)
    {
        var commandLine = context.ParseResult.ToString();

        try
        {
            Logger?.LogTrace("[Command Execution Invocation Started - {ParseResult}]", commandLine);

            await next(context);

            Logger?.LogTrace("[Command Execution Invocation Ended - {ParseResult}]", commandLine);
        }
        catch (Exception error)
        {
            LogException(error);
            Logger?.LogError("[Unhandled Exception - {ParseResult}] - {ExceptionMessage} - {ExceptionStackTrace}",
                commandLine, error.Message, error.StackTrace);
        }
    }

    private static void LogException(Exception error)
    {
        Logger?.LogError("{ExceptionMessage}", error.Message);
        if (error.StackTrace != null)
        {
            Logger?.LogError("{ExceptionStackTrace}", error.StackTrace);
        }

        if (error.InnerException == null)
        {
            return;
        }

        Logger?.LogError("==Inner Exception==");
        LogException(error.InnerException);
        Logger?.LogError("==End Inner Exception==");
    }
}
