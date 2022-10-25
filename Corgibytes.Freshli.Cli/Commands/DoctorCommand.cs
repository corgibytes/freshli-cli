using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Commands;

public class DoctorCommand : RunnableCommand<DoctorCommand, DoctorCommandOptions>
{
    public DoctorCommand(IConfiguration configuration) : base("doctor",
        "Validating environment assumptions.")
    {
        var gitPath = new Option<string>(
            "--git-path",
            description: "Path to the git binary. Default = 'git'",
            getDefaultValue: () => configuration.GitPath
        )
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(gitPath);
    }
}
