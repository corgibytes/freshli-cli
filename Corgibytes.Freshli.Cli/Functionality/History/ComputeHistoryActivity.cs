using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly Guid _analysisId;
    [JsonProperty] private readonly IAnalysisLocation _analysisLocation;
    [JsonProperty] private readonly ICacheDb _cacheDb;
    [JsonProperty] private readonly IComputeHistory _computeHistoryService;
    private readonly string _gitExecutablePath;

    public ComputeHistoryActivity(string gitExecutablePath, ICacheDb cacheDb, IComputeHistory computeHistoryService,
        Guid analysisId, IAnalysisLocation analysisLocation)
    {
        _gitExecutablePath = gitExecutablePath;
        _cacheDb = cacheDb;
        _computeHistoryService = computeHistoryService;
        _analysisId = analysisId;
        _analysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var analysis = _cacheDb.RetrieveAnalysis(_analysisId);
        if (analysis == null)
        {
            return;
        }

        IEnumerable<HistoryIntervalStop> historyIntervalStops;

        if (analysis.UseCommitHistory.Equals(CommitHistory.AtInterval))
        {
            historyIntervalStops = _computeHistoryService
                .ComputeWithHistoryInterval(_analysisLocation, _gitExecutablePath, analysis.HistoryInterval, DateTimeOffset.Now);
        }
        else
        {
            historyIntervalStops =
                _computeHistoryService.ComputeCommitHistory(_analysisLocation, _gitExecutablePath);
        }

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            eventClient.Fire(new HistoryIntervalStopFoundEvent
            {
                GitCommitIdentifier = historyIntervalStop.GitCommitIdentifier,
                AnalysisLocation = _analysisLocation
            });
        }
    }
}
