using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    [JsonProperty] private readonly string _cacheDirectory;
    [JsonProperty] private readonly string _repositoryId;
    [JsonProperty] private readonly string? _commitId;

    public AnalysisLocation(string cacheDirectory, string repositoryId, string? commitId = null)
    {
        _cacheDirectory = cacheDirectory;
        _repositoryId = repositoryId;
        _commitId = commitId;
    }

    public string Path
    {
        get
        {
            if (_commitId == null)
            {
                return System.IO.Path.Combine(_cacheDirectory, "repositories", _repositoryId);
            }

            return System.IO.Path.Combine(_cacheDirectory, "histories", _repositoryId, _commitId);
        }
    }
}
