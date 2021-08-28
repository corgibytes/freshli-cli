using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Corgibytes.Freshli.Cli
{
    public class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger(); 

        public static async Task<int> Main(string[] args)
        {
            CommandLineBuilder cmdBuilder = CreateCommandLineBuilder();
            return await cmdBuilder.UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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

                context.Console.Out.Write(callingMessage);
                s_logger.Trace(callingMessage);

                await next(context);

                context.Console.Out.Write(doneMessage);
                s_logger.Trace(doneMessage);
            }
            catch (Exception e)
            {
                string message = $"[Unhandled Exception - {commandLine}] - {e.Message}";
                context.Console.Out.Write($"{message} - Take a look at the log for detailed information.\n. {e.StackTrace}");
                s_logger.Error($"{message} - {e.StackTrace}");
            }
        }
    }
}
