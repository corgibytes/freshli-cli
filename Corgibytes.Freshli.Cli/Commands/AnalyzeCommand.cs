using System;
using System.CommandLine;
using System.Text.RegularExpressions;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class AnalyzeCommand : RunnableCommand<AnalyzeCommand, AnalyzeCommandOptions>
{
    public AnalyzeCommand() : base("analyze",
        "The primary user-facing command. This command will delegate to other freshli activities to accomplish its work. It will manage work queues to enable parallelization.")
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

        var branch = new Option<string>(
            "--branch",
            description: "The branch to checkout after cloning the repository. If the option is not specified, then no checkout command will be issued. The remote server’s default branch will be used instead."
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
            var regex = new Regex("/\\d+[ymwd]?/");
            if (givenValue != null && !regex.IsMatch(givenValue))
            {
                optionResult.ErrorMessage = $"Option {givenValue} not valid. Valid values are a number followed by a suffix. For example: 2m";
            }
        });
        AddOption(historyInterval);

        var workers = new Option<int>("--workers",
            "The number of worker processes that should be running at any given time. This defaults to twice the number of CPU cores.")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };
        AddOption(workers);

        var repositoryIdentifier =
            new Argument<string>("repository-location", "The location of the repository. This could be either a direct URL or a local directory")
            {
                Arity = ArgumentArity.ExactlyOne
            };
        AddArgument(repositoryIdentifier);
    }
}
