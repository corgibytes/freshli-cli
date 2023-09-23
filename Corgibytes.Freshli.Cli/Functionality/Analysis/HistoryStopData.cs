using System;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class HistoryStopData : IHistoryStopData
{
    public required IConfiguration Configuration { get; init; }
    public string? LocalDirectory { get; init; }
    public required string RepositoryId { get; init; }
    public string? CommitId { get; init; }
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
