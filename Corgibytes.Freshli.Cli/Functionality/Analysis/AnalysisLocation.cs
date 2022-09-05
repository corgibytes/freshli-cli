using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    private readonly string _repositoryId;
    private readonly string _cacheDirectory;
    public readonly GitCommitIdentifier? GitCommitIdentifier;

    public AnalysisLocation(string repositoryId, string cacheDirectory, GitCommitIdentifier? gitCommitIdentifier = null)
    {
        _repositoryId = repositoryId;
        _cacheDirectory = cacheDirectory;
        GitCommitIdentifier = gitCommitIdentifier;
    }

    public string Path => _cacheDirectory + "/repositories/" + _repositoryId;
}
