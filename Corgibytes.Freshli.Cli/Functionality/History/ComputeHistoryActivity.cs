using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : IApplicationActivity
{
    public Guid AnalysisId;

    // ReSharper disable once MemberCanBePrivate.Global
    public readonly IAnalysisLocation AnalysisLocation;

    public readonly string GitExecutablePath;

    public readonly string CacheDir;

    public ComputeHistoryActivity(string gitExecutablePath, string cacheDir,
        Guid analysisId, IAnalysisLocation analysisLocation)
    {
        GitExecutablePath = gitExecutablePath;
        CacheDir = cacheDir;
        AnalysisId = analysisId;
        AnalysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var computeHistoryService = eventClient.ServiceProvider.GetRequiredService<IComputeHistory>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb(CacheDir);
        var analysis = cacheDb.RetrieveAnalysis(AnalysisId);
        if (analysis == null)
        {
            return;
        }

        var historyIntervalStops =
            computeHistoryService.ComputeWithHistoryInterval(AnalysisLocation, GitExecutablePath,
                analysis.HistoryInterval);

        foreach (var historyIntervalStop in historyIntervalStops)
        {
            eventClient.Fire(new HistoryIntervalStopFoundEvent
            {
                GitCommitIdentifier = historyIntervalStop.GitCommitIdentifier,
                AnalysisLocation = AnalysisLocation
            });
        }
    }
}
