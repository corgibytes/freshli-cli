namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class CacheWasNotPreparedEvent : ErrorEvent
{
    public string CacheDirectory { get; init; } = null!;
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
}
