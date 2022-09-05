using System.IO;

using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    [JsonProperty] private string _cacheDirectory;
    [JsonProperty] private string _repositoryId;
    [JsonProperty] private string? _commitId;

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
            else
            {
                return System.IO.Path.Combine(_cacheDirectory, "histories", _repositoryId, _commitId);
            }
        }
    }
}
