using System;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class HistoryStopData : IHistoryStopData
{
    [JsonProperty] private readonly IConfiguration _configuration;

    public HistoryStopData(IConfiguration configuration, string repositoryId, string? commitId = null, DateTimeOffset? moment = null)
    {
        _configuration = configuration;
        RepositoryId = repositoryId;
        CommitId = commitId;
        Moment = moment;
    }

    public string RepositoryId { get; }
    public string? CommitId { get; }
    public DateTimeOffset? Moment { get; }

    public string Path
    {
        get
        {
            if (CommitId == null)
            {
                return System.IO.Path.Combine(_configuration.CacheDir, "repositories", RepositoryId);
            }

            return System.IO.Path.Combine(_configuration.CacheDir, "histories", RepositoryId, CommitId);
        }
    }
}
