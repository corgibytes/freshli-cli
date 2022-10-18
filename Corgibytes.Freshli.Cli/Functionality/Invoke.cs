using System;
using System.IO;
using System.Text;
using CliWrap;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Invoke : IInvoke
{
    private readonly ILogger<Invoke>? _logger;

    public Invoke(ILogger<Invoke>? logger = null)
    {
        _logger = logger;
    }

    public string Command(string executable, string arguments, string workingDirectory)
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        _logger?.LogDebug("Command: " + executable + "; Args: " + arguments + "; WorkingDir: " + workingDirectory);

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
            _logger?.LogError(error.ToString());
            foreach (var innerError in error.InnerExceptions)
            {
                _logger?.LogError(innerError.ToString());
            }
            _logger?.LogError(stdOutBuffer.ToString());
            _logger?.LogError(stdErrBuffer.ToString());

            throw new IOException(stdErrBuffer.ToString());
        }

        return stdOutBuffer.ToString();
    }
}
