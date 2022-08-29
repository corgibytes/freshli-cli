using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly ICacheDb _cacheDb;
    [JsonProperty] private readonly IComputeHistory _computeHistoryService;
    [JsonProperty] private readonly Guid _analysisId;
    [JsonProperty] private readonly string _repositoryId;

    public ComputeHistoryActivity(ICacheDb cacheDb, IComputeHistory computeHistoryService, Guid analysisId, string repositoryId)
    {
        _cacheDb = cacheDb;
        _computeHistoryService = computeHistoryService;
        _analysisId = analysisId;
        _repositoryId = repositoryId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var analysis = _cacheDb.RetrieveAnalysis(_analysisId);
        if (analysis == null)
        {
            return;
        }

        var historyIntervalStops =
            _computeHistoryService.
                ComputeWithHistoryInterval(_repositoryId, analysis.GitPath, analysis.HistoryInterval, analysis.CacheDirectory);

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            eventClient.Fire(new HistoryIntervalStopFoundEvent()
            {
                GitCommitIdentifier = historyIntervalStop.GitCommitIdentifier,
                RepositoryId = _repositoryId
            });
        }
    }
}

