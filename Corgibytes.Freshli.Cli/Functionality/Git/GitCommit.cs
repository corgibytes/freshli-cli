using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitCommit
{
    public readonly DateTimeOffset CommittedAt;
    public readonly string ShaIdentifier;

    public GitCommit(string shaIdentifier, DateTimeOffset committedAt)
    {
        ShaIdentifier = shaIdentifier;
        CommittedAt = committedAt;
    }
}
