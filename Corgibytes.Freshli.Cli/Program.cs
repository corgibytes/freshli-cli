using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.Hosting;
using NLog;
using TextTableFormatter;

namespace Corgibytes.Freshli.Cli;

public class Program
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public static async Task<int> Main(string[] args)
    {
        var cmdBuilder = CreateCommandLineBuilder();
        return await cmdBuilder.UseDefaults()
            .Build()
            .InvokeAsync(args);
    }

    public static CommandLineBuilder CreateCommandLineBuilder()
    {
        var command = new MainCommand
            {
                // Add commands here!
                new ScanCommand(),
                new CacheCommand(),
                new AgentsCommand()
            };


        var builder = new CommandLineBuilder(command)
            .UseHost(CreateHostBuilder)
            .AddMiddleware(async (context, next) =>
            {
                await LogExecution(context, next);
            })
            .UseExceptionHandler()
            .UseHelp();

        return builder;
    }
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            new FreshliServiceBuilder(services).Register();
        });

    public static async Task LogExecution(InvocationContext context, Func<InvocationContext, Task> next)
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
            context.Console.Out.WriteLine($"{message} - Take a look at the log for detailed information.\n. {e.StackTrace}");

            LogException(context.Console.Out, s_logger, e);
            s_logger.Error($"{message} - {e.StackTrace}");
        }
    }

    private static void LogException(IStandardStreamWriter output, Logger logger, Exception error)
    {
        logger.Error(error.Message);
        output.WriteLine(error.Message);
        logger.Error(error.StackTrace);
        output.WriteLine(error.StackTrace);

        if (error.InnerException != null)
        {
            logger.Error("==Inner Exception==");
            output.WriteLine("==Inner Exception==");
            LogException(output, logger, error.InnerException);
            logger.Error("==End Inner Exception==");
            output.WriteLine("==End Inner Exception==");
        }
    }
}
