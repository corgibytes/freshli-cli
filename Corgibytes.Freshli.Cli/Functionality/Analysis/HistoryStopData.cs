using System;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class HistoryStopData : IHistoryStopData
{
    [JsonProperty] private readonly IConfiguration _configuration;

    public HistoryStopData(IConfiguration configuration, string repositoryId, string? commitId = null,
        DateTimeOffset? asOfDate = null)
    {
        _configuration = configuration;
        RepositoryId = repositoryId;
        CommitId = commitId;
        AsOfDate = asOfDate;
    }

    public string? LocalDirectory { get; init; }

    public string RepositoryId { get; }
    public string? CommitId { get; }
    public DateTimeOffset? AsOfDate { get; }

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
