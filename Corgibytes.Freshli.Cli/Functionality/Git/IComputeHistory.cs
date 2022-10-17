using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IComputeHistory
{
    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        IHistoryStopData historyStopData,
        string historyInterval,
        DateTimeOffset startDate
    );

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IHistoryStopData historyStopData);

    public IEnumerable<HistoryIntervalStop> ComputeLatestOnly(IHistoryStopData historyStopData);
}
