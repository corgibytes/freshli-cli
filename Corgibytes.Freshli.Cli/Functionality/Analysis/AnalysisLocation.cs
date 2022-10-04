using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    [JsonProperty] private readonly IConfiguration _configuration;

    public AnalysisLocation(IConfiguration configuration, string repositoryId, string? commitId = null)
    {
        _configuration = configuration;
        RepositoryId = repositoryId;
        CommitId = commitId;
    }

    public string? LocalDirectory { get; init; }

    public string RepositoryId { get; }
    public string? CommitId { get; }

    public string Path
    {
        get
        {
            if (CommitId == null)
            {
                if (LocalDirectory != null)
                {
                    return LocalDirectory;
                }

                return System.IO.Path.Combine(_configuration.CacheDir, "repositories", RepositoryId);
            }

            return System.IO.Path.Combine(_configuration.CacheDir, "histories", RepositoryId, CommitId);
        }
    }
}
