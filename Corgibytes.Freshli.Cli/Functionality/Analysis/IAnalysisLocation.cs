namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public interface IAnalysisLocation
{
    public string? CommitId { get; }

    public string? CacheDirectory { get; }

    public string? RepositoryId { get; }

    public string Path { get; }
}
