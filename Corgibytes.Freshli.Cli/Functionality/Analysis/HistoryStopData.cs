using System;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class HistoryStopData : IHistoryStopData
{
    [JsonProperty] private readonly IConfiguration _configuration;

    public HistoryStopData(IConfiguration configuration, string repositoryId, string? commitId = null,
        DateTimeOffset asOfDateTime = default)
    {
        _configuration = configuration;
        RepositoryId = repositoryId;
        CommitId = commitId;
        AsOfDateTime = asOfDateTime;
    }

    public string? LocalDirectory { get; init; }

    public string RepositoryId { get; }
    public string? CommitId { get; }
    public DateTimeOffset AsOfDateTime { get; }

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
