using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IComputeHistory
{
    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        IAnalysisLocation analysisLocation,
        string gitPath,
        string historyInterval
    );

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IAnalysisLocation analysisLocation, string gitPath);
}
