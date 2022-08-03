using System;
using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands.Git;

public class ComputeHistoryCommand : RunnableCommand<ComputeHistoryCommand, ComputeHistoryCommandOptions>
{
    public ComputeHistoryCommand() : base("compute-history",
        "Used to examine the history of the specified repository and determine the sha hashes that will need to be checked out to complete a historical analysis at the intervals specified.")
    {
        var repositoryIdentifier =
            new Argument<string>("repository-id", "The ID of the repository to compute history for")
            {
                Arity = ArgumentArity.ExactlyOne
            };

        AddArgument(repositoryIdentifier);

        var commitHistory = new Option<bool>("--commit-history",
            "Analyzes only the points in time when files have changed in the commit history")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };
        AddOption(commitHistory);

        var historyInterval = new Option<string>("--history-interval",
            description:
            "As the analyze command moves backwards in time through the history of the project, what time interval should it use to determine the points in time that are sampled. Possible values: y(ears), m(onths), w(eeks), d(ays)",
            getDefaultValue: () => "m")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ZeroOrOne
        };

        historyInterval.AddValidator(optionResult =>
        {
            var givenValue = optionResult.GetValueOrDefault<string>();
            string[] possibleValues = { "y", "m", "w", "d" };

            if (!Array.Exists(possibleValues, possibleValue => possibleValue == givenValue))
            {
                optionResult.ErrorMessage = string.Format(
                    "Option {0} not valid. Possible options are {1}",
                    givenValue,
                    string.Join(", ", possibleValues)
                );
            }
        });

        AddOption(historyInterval);

        var gitPath = new Option<string>("--git-path",
            description: "Path to the git binary. Default = 'git'", getDefaultValue: () => "git")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        AddOption(gitPath);
    }
}
