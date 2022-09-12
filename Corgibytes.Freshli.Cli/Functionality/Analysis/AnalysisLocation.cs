using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    [JsonProperty] public string CacheDirectory { get; }
    [JsonProperty] public string RepositoryId { get; }
    [JsonProperty] public string? CommitId { get; }

    public AnalysisLocation(string cacheDirectory, string repositoryId, string? commitId = null)
    {
        CacheDirectory = cacheDirectory;
        RepositoryId = repositoryId;
        CommitId = commitId;
    }

    public string Path
    {
        get
        {
            if (CommitId == null)
            {
                return System.IO.Path.Combine(CacheDirectory, "repositories", RepositoryId);
            }

            return System.IO.Path.Combine(CacheDirectory, "histories", RepositoryId, CommitId);
        }
    }
}
