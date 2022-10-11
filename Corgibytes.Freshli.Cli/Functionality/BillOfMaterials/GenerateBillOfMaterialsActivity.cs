using System;
using System.IO;
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

        var asOfDateTime = DateTime.Now;
        var pathToBillOfMaterials =
            agentReader.ProcessManifest(Path.Combine(historyStopPoint?.LocalPath!, ManifestPath), asOfDateTime);

        eventClient.Fire(new BillOfMaterialsGeneratedEvent(AnalysisId, HistoryStopPointId, pathToBillOfMaterials,
            AgentExecutablePath));
    }
}
