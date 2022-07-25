using System.IO;
using System.Reflection;
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

    // This will be needed in later work, but is being defined now, so we'll disable the warning about being unused.
    // ReSharper disable once UnusedMember.Global
    public static string Freshli(string arguments)
    {
        var executionLocation = new FileInfo(
            Assembly.GetExecutingAssembly().Location
        ).Directory!; // null is forgivable here, because if we get null, there are far weirder problems afoot
        var executable = new FileInfo(executionLocation.FullName + "/freshli");

        try
        {
            return Command(executable.FullName, arguments);
        }
        catch (IOException e)
        {
            throw new IOException(
                $"Invoking 'freshli {arguments}' failed with the following output:\n{e.Message}"
            );
        }
    }
}
