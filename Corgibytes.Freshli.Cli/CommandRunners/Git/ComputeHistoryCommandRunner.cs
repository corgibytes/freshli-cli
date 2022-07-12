using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class ComputeHistoryCommandRunner : CommandRunner<ComputeHistoryCommand, ComputeHistoryCommandOptions>
{
    private readonly ComputeHistory _computeHistory;

    public ComputeHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner, ComputeHistory computeHistory) : base(serviceProvider, runner)
    {
        _computeHistory = computeHistory;
    }

    public override int Run(ComputeHistoryCommandOptions options, InvocationContext context)
    {
        foreach (var historyIntervalStop in
                 _computeHistory.ComputeWithHistoryInterval(options.RepositoryId, options.GitPath.FullName, options.HistoryInterval))
        {
            context.Console.WriteLine(
                historyIntervalStop.CommittedAt.ToString(CultureInfo.InvariantCulture) + " " + historyIntervalStop.GitCommitIdentifier
            );
        }

        return 0;
    }
}
