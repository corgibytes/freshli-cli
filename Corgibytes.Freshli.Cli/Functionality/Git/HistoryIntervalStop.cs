using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class HistoryIntervalStop
{
    private readonly string _gitCommitIdentifier;
    private readonly DateTimeOffset _stopDate;

    public HistoryIntervalStop(DateTimeOffset stopDate, string gitCommitIdentifier)
    {
        _stopDate = stopDate;
        _gitCommitIdentifier = gitCommitIdentifier;
    }
}
