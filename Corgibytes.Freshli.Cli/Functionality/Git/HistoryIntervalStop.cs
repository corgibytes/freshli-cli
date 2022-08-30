using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class HistoryIntervalStop
{
    public HistoryIntervalStop(string gitCommitIdentifier, DateTimeOffset committedAt)
    {
        GitCommitIdentifier = gitCommitIdentifier;
        CommittedAt = committedAt;
    }

    public string GitCommitIdentifier { get; }
    public DateTimeOffset CommittedAt { get; }
}
