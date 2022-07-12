using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class HistoryIntervalStop
{
    public string GitCommitIdentifier { get; }
    public DateTimeOffset CommittedAt { get; }

    public HistoryIntervalStop(DateTimeOffset committedAt, string gitCommitIdentifier)
    {
        CommittedAt = committedAt;
        GitCommitIdentifier = gitCommitIdentifier;
    }
}
