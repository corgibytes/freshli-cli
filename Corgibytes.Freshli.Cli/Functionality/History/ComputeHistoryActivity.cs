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

    public readonly string GitExecutablePath;
    public Guid AnalysisId;

    public ComputeHistoryActivity(string gitExecutablePath, Guid analysisId,
        IAnalysisLocation analysisLocation)
    {
        GitExecutablePath = gitExecutablePath;
        AnalysisId = analysisId;
        AnalysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var computeHistoryService = eventClient.ServiceProvider.GetRequiredService<IComputeHistory>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb(AnalysisLocation.CacheDirectory);
        var analysis = cacheDb.RetrieveAnalysis(AnalysisId);
        if (analysis == null)
        {
            return;
        }

        IEnumerable<HistoryIntervalStop> historyIntervalStops;

        if (analysis.UseCommitHistory.Equals(CommitHistory.AtInterval))
        {
            historyIntervalStops = computeHistoryService
                .ComputeWithHistoryInterval(AnalysisLocation, GitExecutablePath, analysis.HistoryInterval, DateTimeOffset.Now);
        }
        else
        {
            historyIntervalStops =
                computeHistoryService.ComputeCommitHistory(AnalysisLocation, GitExecutablePath);
        }

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            eventClient.Fire(new HistoryIntervalStopFoundEvent
            {
                GitExecutablePath = GitExecutablePath,
                GitCommitIdentifier = historyIntervalStop.GitCommitIdentifier,
                AnalysisLocation = AnalysisLocation
            });
        }
    }
}
