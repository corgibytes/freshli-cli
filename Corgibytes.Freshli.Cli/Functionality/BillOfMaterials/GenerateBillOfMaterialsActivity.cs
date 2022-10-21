using System;
using System.Collections.Concurrent;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity
{
    public readonly string AgentExecutablePath;
    public readonly Guid AnalysisId;
    public readonly int HistoryStopPointId;
    public readonly string ManifestPath;

    private static readonly ConcurrentDictionary<string, object> s_lockPoints = new();

    public GenerateBillOfMaterialsActivity(Guid analysisId, string agentExecutablePath,
        int historyStopPointId, string manifestPath)
    {
        AnalysisId = analysisId;
        AgentExecutablePath = agentExecutablePath;
        HistoryStopPointId = historyStopPointId;
        ManifestPath = manifestPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        _ = historyStopPoint ?? throw new Exception($"Failed to retrieve history stop point {HistoryStopPointId}");

        var historyPointPath = historyStopPoint.LocalPath;
        var asOfDateTime = historyStopPoint.AsOfDateTime;

        EnsureLockPointExists(historyPointPath);
        lock (s_lockPoints[historyPointPath])
        {
            var fullManifestPath = Path.Combine(historyPointPath, ManifestPath);
            var bomFilePath = agentReader.ProcessManifest(fullManifestPath, asOfDateTime);
            var cachedBomFilePath = cacheManager.StoreBomInCache(bomFilePath, AnalysisId, asOfDateTime);

            eventClient.Fire(new BillOfMaterialsGeneratedEvent(
                AnalysisId, HistoryStopPointId, cachedBomFilePath, AgentExecutablePath));
        }
    }

    private void EnsureLockPointExists(string path)
    {
        if (!s_lockPoints.ContainsKey(path))
        {
            s_lockPoints[path] = new object();
        }
    }
}
