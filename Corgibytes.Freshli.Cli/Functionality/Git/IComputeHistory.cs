using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IComputeHistory
{
    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        IAnalysisLocation analysisLocation,
        string historyInterval,
        DateTimeOffset startDate
    );

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IAnalysisLocation analysisLocation);

    public IEnumerable<HistoryIntervalStop> ComputeLatestOnly(IAnalysisLocation analysisLocation);
}
