using System;
using System.IO;
using System.Text;
using CliWrap;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CommandInvoker : ICommandInvoker
{
    private readonly ILogger<CommandInvoker>? _logger;

    public CommandInvoker(ILogger<CommandInvoker>? logger = null) => _logger = logger;

    public string Run(string executable, string arguments, string workingDirectory)
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        _logger?.LogDebug(
            "Command: {Executable}; Args: {Arguments}; WorkingDir: {WorkingDir}",
            executable,
            arguments,
            workingDirectory
        );

        var command = CliWrap.Cli.Wrap(executable).WithArguments(
                args => args
                    .Add(arguments.Split())
            )
            .WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        try
        {
            using var task = command.ExecuteAsync().Task;
            task.Wait();
        }
        catch (AggregateException error)
        {
            _logger?.LogError("{Exception}", error.ToString());
            foreach (var innerError in error.InnerExceptions)
            {
                _logger?.LogError("{InnerException}", innerError.ToString());
            }

            _logger?.LogError("{StdOutBuffer}", stdOutBuffer.ToString());
            _logger?.LogError("{StdErrBuffer}", stdErrBuffer.ToString());

            throw new IOException(stdErrBuffer.ToString());
        }

        return stdOutBuffer.ToString();
    }
}
