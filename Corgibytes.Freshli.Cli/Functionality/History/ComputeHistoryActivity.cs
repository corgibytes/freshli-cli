using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : IApplicationActivity
{
    public readonly IAnalysisLocation AnalysisLocation;

    public Guid AnalysisId;

    public ComputeHistoryActivity(Guid analysisId, IAnalysisLocation analysisLocation)
    {
        AnalysisId = analysisId;
        AnalysisLocation = analysisLocation;
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

        if (analysis.UseCommitHistory.Equals(CommitHistory.AtInterval))
        {
            historyIntervalStops = computeHistoryService
                .ComputeWithHistoryInterval(AnalysisLocation, analysis.HistoryInterval, DateTimeOffset.Now);
        }
        else
        {
            historyIntervalStops = computeHistoryService.ComputeCommitHistory(AnalysisLocation);
        }

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            var historyStopLocation =
                new AnalysisLocation(configuration, AnalysisLocation.RepositoryId, historyIntervalStop.GitCommitIdentifier);

            eventClient.Fire(new HistoryIntervalStopFoundEvent
            {
                AnalysisLocation = historyStopLocation
            });
        }
    }
}
