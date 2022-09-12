using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    [JsonProperty] private readonly string _cacheDirectory;
    [JsonProperty] private readonly string _repositoryId;
    [JsonProperty] public string? CommitId { get; }

    public AnalysisLocation(string cacheDirectory, string repositoryId, string? commitId = null)
    {
        _cacheDirectory = cacheDirectory;
        _repositoryId = repositoryId;
        CommitId = commitId;
    }

    public string Path
    {
        get
        {
            if (CommitId == null)
            {
                return System.IO.Path.Combine(_cacheDirectory, "repositories", _repositoryId);
            }

            return System.IO.Path.Combine(_cacheDirectory, "histories", _repositoryId, CommitId);
        }
    }
}
