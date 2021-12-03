using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Extensions.Logging;
using NLog.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.String;

namespace Corgibytes.Freshli.Cli
{
    public class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private const string DefaultLogLayout = "${date}|${level:uppercase=true:padding=5}|${logger}:${callsite-linenumber}|${message} ${exception}";

        public static async Task<int> Main(string[] args)
        {
            CommandLineBuilder cmdBuilder = CreateCommandLineBuilder();
            return await cmdBuilder.UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        private static void ConfigureLogging(string consoleLogLevel, string logfile)
        {
            NLog.LogLevel desiredLevel = NLog.LogLevel.FromString(consoleLogLevel);

            LoggingConfiguration config = new LoggingConfiguration();
            if (IsNullOrEmpty(logfile))
            {
                ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
                config.AddTarget("console", consoleTarget);
                consoleTarget.Layout = DefaultLogLayout;
                config.LoggingRules.Add(new LoggingRule("*", desiredLevel, consoleTarget));
            }
            else
            {
                FileTarget fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);
                fileTarget.FileName = logfile;
                fileTarget.Layout = DefaultLogLayout;
                config.LoggingRules.Add(new LoggingRule("*", desiredLevel, fileTarget));
            }

            LogManager.Configuration = config;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddNLog();
                })
                .UseNLog()
                .ConfigureServices((_, services) =>
            {
                new FreshliServiceBuilder(services).Register();
            });

        public static CommandLineBuilder CreateCommandLineBuilder()
        {
            Option logLevelOption = new Option<string>(alias: "--loglevel", description:"the minimum log level to log to console",
                getDefaultValue: () => "Warn");
            Option logFileOption = new Option<string?>("--logfile", "file for logs instead of logging to console");

            CommandLineBuilder builder = new CommandLineBuilder(new MainCommand())
                .UseHost(CreateHostBuilder)
                .UseExceptionHandler()
                .UseHelp()
                .AddCommand(new ScanCommand())
                .AddOption(logLevelOption)
                .AddOption(logFileOption);
            builder.UseMiddleware(context=>
            {
                ConfigureLogging(context.ParseResult.ValueForOption<string>(logLevelOption),
                    context.ParseResult.ValueForOption<string>(logFileOption));

            });
            builder.UseMiddleware(async (context, next) =>
            {
                await LogExecution(context, next);
            });
            return builder;
        }

        public static async Task LogExecution(InvocationContext context, Func<InvocationContext, Task> next)
        {
            string commandLine = context.ParseResult.ToString();

            try
            {
                string callingMessage = $"[Command Execution Invocation Started  - {commandLine} ]\n";
                string doneMessage = $"[Command Execution Invocation Ended - {commandLine} ]\n";

                s_logger.Trace(callingMessage);

                await next(context);

                s_logger.Trace(doneMessage);
            }
            catch (Exception e)
            {
                string message = $"[Unhandled Exception - {commandLine}] - {e.Message}";
                s_logger.Error($"{message} - {e.StackTrace}");
            }
        }
    }
}
