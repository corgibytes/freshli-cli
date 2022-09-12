using System;

// Currently the public fields aren't being used but once we get the API call to submit the results to we'll need these to be public
// Related: https://github.com/corgibytes/freshli-cli/issues/235
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

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
