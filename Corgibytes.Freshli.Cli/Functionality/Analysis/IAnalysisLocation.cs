// ReSharper disable UnusedMemberInSuper.Global

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public interface IAnalysisLocation
{
    public string RepositoryId { get; }
    public string? CommitId { get; }
    public string Path { get; }
}
