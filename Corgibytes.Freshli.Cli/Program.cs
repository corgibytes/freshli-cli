using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.IoC;
using Corgibytes.Freshli.Cli.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using Spectre.Console;
using static System.String;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;
using LogLevel = NLog.LogLevel;

namespace Corgibytes.Freshli.Cli;

public class Program
{
    private const string DefaultLogLayout =
        "${date} | " +
        "${level:uppercase=true:padding=5} | " +
        "${logger}:${callsite-linenumber} | " +
        "${message} ${exception}";

    private ILogger<Program>? Logger { get; set; }
    private IConfiguration Configuration { get; } = new Configuration(new Environment());
    private List<QueuedHostedService> Workers { get; } = new();

    public static async Task<int> Main(params string[] args)
    {
        var program = new Program();
        var cmdBuilder = program.CreateCommandLineBuilder();
        var parser = cmdBuilder.UseDefaults().Build();
        return await parser.InvokeAsync(args);
    }

    private static void ConfigureLogging(IServiceProvider serviceProvider, string? consoleLogLevel, string? logfile)
    {
        var desiredLevel = LogLevel.FromString(consoleLogLevel);

        var config = new LoggingConfiguration();
        if (IsNullOrEmpty(logfile))
        {
            var ansiConsole = serviceProvider.GetRequiredService<IAnsiConsole>();
            var consoleTarget = new SpectreAnsiConsoleTarget(ansiConsole);
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

    private IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddNLog();
            })
            .ConfigureServices((_, services) =>
            {
                new FreshliServiceBuilder(services, Configuration).Register();
                Logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
            });
    }


    public CommandLineBuilder CreateCommandLineBuilder()
    {
        var builder = new CommandLineBuilder(new MainCommand(Configuration))
            .UseHost(CreateHostBuilder)
            .AddMiddleware(async (context, next) =>
            {
                var host = context.BindingContext.GetRequiredService<IHost>();

                await Task.Run(() => ConfigureLogging(
                    host.Services,
                    context.ParseResult.GetOptionValueByName<string>("loglevel"),
                    context.ParseResult.GetOptionValueByName<string>("logfile"))
                );

                await next(context);
            })
            .AddMiddleware(LogExecution)
            .AddMiddleware(async (context, next) =>
            {
                ParseWorkersOption(context);

                Logger!.LogDebug("Starting workers. Worker count: {WorkerCount}", Configuration.WorkerCount);

                var host = context.BindingContext.GetRequiredService<IHost>();

                var startTasks = new List<Task>();
                while (Workers.Count < Configuration.WorkerCount)
                {
                    var worker = ActivatorUtilities.CreateInstance<QueuedHostedService>(host.Services);
                    Workers.Add(worker);

                    var startTask = worker.StartAsync(context.GetCancellationToken());
                    startTasks.Add(startTask);
                }

                await next(context);

                var stopTasks = new List<Task>();
                while (Workers.Count > 0)
                {
                    var worker = Workers[0];

                    var stopTask = worker.StopAsync(context.GetCancellationToken());
                    stopTasks.Add(stopTask);
                    Workers.Remove(worker);
                }

                Task.WaitAll(stopTasks.ToArray(), context.GetCancellationToken());
                Task.WaitAll(startTasks.ToArray(), context.GetCancellationToken());
            })
            .UseExceptionHandler()
            .CancelOnProcessTermination()
            .UseHelp();

        return builder;
    }

    private void ParseWorkersOption(InvocationContext context)
    {
        var workerCount = context.ParseResult.GetOptionValueByName<int>("workers");

        if (workerCount == 0)
        {
            workerCount = System.Environment.ProcessorCount;
        }

        Configuration.WorkerCount = workerCount;
    }

    private async Task LogExecution(InvocationContext context, Func<InvocationContext, Task> next)
    {
        var commandLine = context.ParseResult.ToString();

        try
        {
            Logger?.LogTrace("[Command Execution Invocation Started - {ParseResult}]", commandLine);

            await next(context);

            Logger?.LogTrace("[Command Execution Invocation Ended - {ParseResult}]", commandLine);
        }
        catch(OperationCanceledException) {
            Logger?.LogWarning("Cancel requested. Exiting...");
        }
        catch(Exception error)
        {
            LogException(error);
            Logger?.LogError("[Unhandled Exception - {ParseResult}] - {ExceptionMessage} - {ExceptionStackTrace}",
                commandLine, error.Message, error.StackTrace);
        }
    }

    private void LogException(Exception error)
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
