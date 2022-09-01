namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    private readonly string _cacheDirectory;
    private readonly string _repositoryId;

    public AnalysisLocation(string repositoryId, string cacheDirectory)
    {
        _repositoryId = repositoryId;
        _cacheDirectory = cacheDirectory;
    }

    public string Path => _cacheDirectory + "/repositories/" + _repositoryId;
}
