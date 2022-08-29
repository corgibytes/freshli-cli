using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IComputeHistory
{
    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(string repositoryId, string gitPath, string cacheDir);

    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        string repositoryId,
        string gitPath,
        string historyInterval,
        string cacheDir
    );
}
