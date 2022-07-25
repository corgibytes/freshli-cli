using System.IO;
using System.Text;
using CliWrap;

namespace Corgibytes.Freshli.Cli.Functionality;

public static class Invoke
{
    public static string Command(string executable, string arguments, string workingDirectory)
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        var command = CliWrap.Cli.Wrap(executable).WithArguments(
                args => args
                    .Add(arguments.Split())
            )
            .WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        if (task.Result.ExitCode != 0)
        {
            throw new IOException(stdErrBuffer.ToString());
        }

        return stdOutBuffer.ToString();
    }
}
