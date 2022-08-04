using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitCommit
{
    public GitCommit(string shaIdentifier, DateTimeOffset committedAt)
    {
        CommittedAt = committedAt;
        ShaIdentifier = shaIdentifier;
    }

    public DateTimeOffset CommittedAt { get; }
    public string ShaIdentifier { get; }
}
