using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class ComputeHistoryCommandRunner : CommandRunner<ComputeHistoryCommand, ComputeHistoryCommandOptions>
{
    private readonly IComputeHistory _computeHistory;

    public ComputeHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner,
        IComputeHistory computeHistory) :
        base(serviceProvider, runner) => _computeHistory = computeHistory;

    public override int Run(ComputeHistoryCommandOptions options, InvocationContext context)
    {
        if (options.CommitHistory)
        {
            WriteStopsToLines(
                _computeHistory.ComputeCommitHistory(new AnalysisLocation(options.CacheDir, options.RepositoryId),
                    options.GitPath),
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
                .ComputeWithHistoryInterval(new AnalysisLocation(options.CacheDir, options.RepositoryId),
                    options.GitPath, historyIntervalDuration),
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
