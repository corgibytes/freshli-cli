using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitCommit
{
    public DateTimeOffset CommittedAt { get; }
    public string ShaIdentifier { get; } 

    public GitCommit(string shaIdentifier, DateTimeOffset committedAt)
    {
        ShaIdentifier = shaIdentifier;
        CommittedAt = committedAt;
    }
}
