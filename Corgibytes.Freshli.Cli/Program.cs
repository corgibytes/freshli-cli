using System;
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

namespace Corgibytes.Freshli.Cli
{
    public class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static async Task<int> Main(string[] args)
        {
            //ConfigureLogging();
            CommandLineBuilder cmdBuilder = CreateCommandLineBuilder();
            return await cmdBuilder.UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        private static void ConfigureLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = "${date}|${level:uppercase=true:padding=5}|${logger}:${callsite-linenumber}|${message} ${exception}";
            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Warn, consoleTarget));

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/freshli.log";
            fileTarget.Layout = "${date}|${level:uppercase=true:padding=5}|${logger}:${callsite-linenumber}|${message} ${exception}";
            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, fileTarget));


            LogManager.Configuration = config;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddNLog();
                    ConfigureLogging();

                }).UseNLog()
                .ConfigureServices((_, services) =>
            {
                new FreshliServiceBuilder(services).Register();
            });

        public static CommandLineBuilder CreateCommandLineBuilder()
        {
            CommandLineBuilder builder = new CommandLineBuilder(new MainCommand())
                .UseHost(CreateHostBuilder)
                .UseMiddleware(async (context, next) =>
                {
                    await LogExecution(context, next);
                })
                .UseExceptionHandler()
               .UseHelp()
               .AddCommand(new ScanCommand());
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
