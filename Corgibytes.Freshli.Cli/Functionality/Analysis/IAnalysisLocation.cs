namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public interface IAnalysisLocation
{
    public string? CommitId { get; }

    public string Path { get; }
}
