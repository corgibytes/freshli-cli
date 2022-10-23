using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity
{
    public DetectManifestsUsingAgentActivity(Guid analysisId, int historyStopPointId,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);

        var manifestPaths = cacheDb.RetrieveCachedManifests(HistoryStopPointId, AgentExecutablePath);

        if (manifestPaths.Count == 0)
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath);
            manifestPaths = agentReader.DetectManifests(historyStopPoint?.LocalPath!);
            cacheDb.StoreCachedManifests(HistoryStopPointId, AgentExecutablePath, manifestPaths);
        }

        foreach (var manifestPath in manifestPaths)
        {
            eventClient.Fire(new ManifestDetectedEvent(AnalysisId, HistoryStopPointId, AgentExecutablePath,
                manifestPath));
        }
    }
}
