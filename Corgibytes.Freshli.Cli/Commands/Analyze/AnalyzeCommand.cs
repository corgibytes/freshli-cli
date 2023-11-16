using System.CommandLine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.Support;

namespace Corgibytes.Freshli.Cli.Commands.Analyze;

public class AnalyzeCommand : RunnableCommand<AnalyzeCommand, AnalyzeCommandOptions>
{
    public AnalyzeCommand(IConfiguration configuration) : base("analyze",
        "The primary user-facing command. This command will delegate to other freshli activities to accomplish its work. It will manage work queues to enable parallelization.")
    {
        var projectSlug = new Option<string>(
            "--project",
            description:
            "The name of the project to associate the analysis with. Takes the form of `organization-nickname/project-nickname`. If not specified, then an attempt will be made to determine which project to use. However, if more than project is present, then an error will be generated.")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(projectSlug);

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

        var branch = new Option<string>(
            "--branch",
            "The branch to checkout after cloning the repository. If the option is not specified, then no checkout command will be issued. The remote server’s default branch will be used instead."
        )
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(branch);

        var commitHistory = new Option<bool>("--commit-history",
            "Analyzes only the points in time when files have changed in the commit history")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };
        AddOption(commitHistory);

        var historyInterval = new Option<string>("--history-interval",
            description:
            "As the analyze command moves backwards in time through the history of the project, what time interval should it use to determine the points in time that are sampled. It's value should be a number followed by a suffix. Possible suffixes: y(ears), m(onths), w(eeks), d(ays)",
            getDefaultValue: () => "1m")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };

        historyInterval.AddValidator(optionResult =>
        {
            var givenValue = optionResult.GetValueOrDefault<string>();
            if (givenValue != null && !new HistoryIntervalParser().IsValid(givenValue))
            {
                optionResult.ErrorMessage =
                    $"Option {givenValue} not valid. Valid values are a number followed by a suffix. For example: 2m";
            }
        });
        AddOption(historyInterval);

        var latestOnly = new Option<bool>("--latest-only",
            "When this is set, analyze will not walk back in history, and overwrites the options --commit-history, --history-interval")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };
        AddOption(latestOnly);

        var repositoryLocation =
            new Argument<string>("repository-location",
                "The location of the repository. This could be either a direct URL or a local directory")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
        AddArgument(repositoryLocation);
    }
}
