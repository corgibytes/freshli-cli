using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Logging;
using Polly;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CommandInvoker : ICommandInvoker
{
    private readonly ILogger<CommandInvoker>? _logger;

    public CommandInvoker(ILogger<CommandInvoker>? logger = null) => _logger = logger;

    public async ValueTask<string> Run(string executable, string arguments, string workingDirectory, int maxRetries = 0)
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
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    maxRetries,
                    // delay progression: 2s, 4s, 8s, 16s, 32s
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (_, retryCount) =>
                {
                    _logger?.LogWarning(
                        "Command failed. Retry {RetryCount} of {MaxRetries}. Command: {Executable}; " +
                        "Args: {Arguments}; WorkingDir: {WorkingDir}",
                        retryCount,
                        maxRetries,
                        executable,
                        arguments,
                        workingDirectory
                    );
                })
                .ExecuteAsync(async () => await command.ExecuteAsync());
        }
        catch (Exception error)
        {
            _logger?.LogError(
                "Command failed. No more retries. Command: {Executable}; Args: {Arguments}; WorkingDir: {WorkingDir}; " +
                "Exception: {Exception}",
                executable,
                arguments,
                workingDirectory,
                error.ToString()
            );

            if (error is AggregateException aggregate)
            {
                foreach (var innerError in aggregate.InnerExceptions)
                {
                    _logger?.LogError("{InnerException}", innerError.ToString());
                }
            }

            _logger?.LogError("{StdOutBuffer}", stdOutBuffer.ToString());
            _logger?.LogError("{StdErrBuffer}", stdErrBuffer.ToString());

            throw;
        }

        return stdOutBuffer.ToString();
    }
}
