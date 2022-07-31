using System;
using System.Collections.Generic;
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

    public ComputeHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner, ComputeHistory computeHistory) :
        base(serviceProvider, runner) => _computeHistory = computeHistory;

    public override int Run(ComputeHistoryCommandOptions options, InvocationContext context)
    {
        if (options.CommitHistory)
        {
            WriteStopsToLines(_computeHistory.ComputeCommitHistory(options.RepositoryId, options.GitPath, options.CacheDir),
                context);
            return 0;
        }


        var historyIntervalDuration = options.HistoryInterval switch
        {
            "d" => "day",
            "w" => "week",
            "m" => "month",
            "y" => "year",
            _ => ""
        };

        WriteStopsToLines(
            _computeHistory
                .ComputeWithHistoryInterval(options.RepositoryId, options.GitPath, historyIntervalDuration, options.CacheDir),
            context
        );

        return 0;
    }

    private static void WriteStopsToLines(IEnumerable<HistoryIntervalStop> historyIntervalStops,
        InvocationContext context)
    {
        foreach (var historyIntervalStop in historyIntervalStops)
        {
            context.Console.WriteLine(
                historyIntervalStop.CommittedAt.ToString(CultureInfo.InvariantCulture) + " " +
                historyIntervalStop.GitCommitIdentifier
            );
        }
    }
}
