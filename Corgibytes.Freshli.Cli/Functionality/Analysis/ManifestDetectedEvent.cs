using System;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : IApplicationEvent
{
    public ManifestDetectedEvent(Guid analysisId, IHistoryStopData historyStopData, string agentExecutablePath,
        string manifestPath)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
        AgentExecutablePath = agentExecutablePath;
        ManifestPath = manifestPath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopData HistoryStopData { get; }
    public string AgentExecutablePath { get; }
    public string ManifestPath { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new GenerateBillOfMaterialsActivity(
            AnalysisId,
            AgentExecutablePath,
            HistoryStopData,
            ManifestPath
        ));
}
