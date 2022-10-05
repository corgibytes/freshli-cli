using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class DoctorCommand : RunnableCommand<DoctorCommand, DoctorCommandOptions>
{
    public DoctorCommand() : base("doctor",
        "Validating environment assumptions.")
    {
        var gitPath = new Option<string>(
            "--git-path",
            description: "Path to the git binary. Default = 'git'",
            getDefaultValue: () => "git"
        )
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(gitPath);
    }
}
