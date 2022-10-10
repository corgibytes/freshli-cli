using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
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
        var cachedAnalysis = cacheDb.RetrieveAnalysis(AnalysisId);

        if (cachedAnalysis == null)
        {
            eventClient.Fire(new AnalysisIdNotFoundEvent());
            return;
        }

        IEnumerable<HistoryIntervalStop> historyIntervalStops;

        if (cachedAnalysis.RevisionHistoryMode.Equals(RevisionHistoryMode.OnlyLatestRevision))
        {
            historyIntervalStops = computeHistoryService.ComputeLatestOnly(AnalysisLocation);
        }
        else if (cachedAnalysis.UseCommitHistory.Equals(CommitHistory.AtInterval))
        {
            try
            {
                historyIntervalStops = computeHistoryService
                    .ComputeWithHistoryInterval(AnalysisLocation, cachedAnalysis.HistoryInterval, DateTimeOffset.Now);
            }
            catch (InvalidHistoryIntervalException exception)
            {
                eventClient.Fire(new InvalidHistoryIntervalEvent{ErrorMessage = exception.Message});
                return;
            }
        }
        else
        {
            historyIntervalStops = computeHistoryService.ComputeCommitHistory(AnalysisLocation);
        }

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            var historyIntervalStopId = cacheDb.AddHistoryIntervalStop(
                new CachedHistoryIntervalStop
                {
                    CachedAnalysisId = AnalysisId,
                    GitCommitId = historyIntervalStop.GitCommitIdentifier,
                    GitCommitDate = historyIntervalStop.CommittedAt
                });

            var historyStopLocation =
                new AnalysisLocation(configuration, AnalysisLocation.RepositoryId,
                    historyIntervalStop.GitCommitIdentifier, historyIntervalStopId);

            eventClient.Fire(new HistoryIntervalStopFoundEvent
            {
                AnalysisId = AnalysisId,
                AnalysisLocation = historyStopLocation
            });
        }
    }
}
