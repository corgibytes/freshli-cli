using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : IApplicationActivity
{
    public readonly IHistoryStopData HistoryStopData;

    public Guid AnalysisId;

    public ComputeHistoryActivity(Guid analysisId, IHistoryStopData historyStopData)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        var computeHistoryService = eventClient.ServiceProvider.GetRequiredService<IComputeHistory>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var analysis = cacheDb.RetrieveAnalysis(AnalysisId);
        if (analysis == null)
        {
            return;
        }

        IEnumerable<HistoryIntervalStop> historyIntervalStops;

        if (analysis.RevisionHistoryMode.Equals(RevisionHistoryMode.OnlyLatestRevision))
        {
            historyIntervalStops = computeHistoryService.ComputeLatestOnly(HistoryStopData);
        }
        else if (analysis.UseCommitHistory.Equals(CommitHistory.AtInterval))
        {
            historyIntervalStops = computeHistoryService
                .ComputeWithHistoryInterval(HistoryStopData, analysis.HistoryInterval, DateTimeOffset.Now);
        }
        else
        {
            historyIntervalStops = computeHistoryService.ComputeCommitHistory(HistoryStopData);
        }

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            var historyStopData =
                new HistoryStopData(configuration, HistoryStopData.RepositoryId,
                    historyIntervalStop.GitCommitIdentifier, historyIntervalStop.CommittedAt);

            eventClient.Fire(new HistoryIntervalStopFoundEvent(AnalysisId, historyStopData));
        }
    }
}
