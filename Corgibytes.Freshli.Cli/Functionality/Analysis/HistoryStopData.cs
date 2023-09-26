using System;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class HistoryStopData : IHistoryStopData
{
    public required IConfiguration Configuration { get; init; }
    public string? LocalDirectory { get; init; }
    public required string RepositoryId { get; init; }
    // TODO: The commit id should still be set, even if we're analyzing a local directory
    public string? CommitId { get; init; }
    // TODO: The commit date should still be set, even if we're analyzing a local directory
    public DateTimeOffset? CommittedAt { get; init; }
    public DateTimeOffset AsOfDateTime { get; init; }

    public string Path
    {
        get
        {
            if (CommitId != null)
            {
                return System.IO.Path.Combine(Configuration.CacheDir, "histories", RepositoryId, CommitId);
            }

            return LocalDirectory ?? System.IO.Path.Combine(Configuration.CacheDir, "repositories", RepositoryId);
        }
    }
}
