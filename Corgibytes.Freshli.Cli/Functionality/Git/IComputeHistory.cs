using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IComputeHistory
{
    // TODO: Make this method return an async-friendly enumerable
    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        IHistoryStopData historyStopData,
        string historyInterval,
        DateTimeOffset startDate
    );

    // TODO: Make this method return an async-friendly enumerable
    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IHistoryStopData historyStopData);

    // TODO: Make this method return an async-friendly enumerable
    public IEnumerable<HistoryIntervalStop> ComputeLatestOnly(IHistoryStopData historyStopData);
}
