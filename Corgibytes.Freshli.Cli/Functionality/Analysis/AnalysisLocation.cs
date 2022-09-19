namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    public AnalysisLocation(string cacheDirectory, string repositoryId, string? commitId = null)
    {
        CacheDirectory = cacheDirectory;
        RepositoryId = repositoryId;
        CommitId = commitId;
    }

    public string CacheDirectory { get; }
    public string RepositoryId { get; }
    public string? CommitId { get; }

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
