using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public ManifestDetectedEvent(Guid analysisId, int historyStopPointId, string agentExecutablePath,
        string manifestPath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        AgentExecutablePath = agentExecutablePath;
        ManifestPath = manifestPath;
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string AgentExecutablePath { get; }
    public string ManifestPath { get; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        try
        {
            await eventClient.Dispatch(new GenerateBillOfMaterialsActivity(
                AnalysisId,
                AgentExecutablePath,
                HistoryStopPointId,
                ManifestPath
            ));
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(new FireHistoryStopPointProcessingErrorActivity(HistoryStopPointId, error));
        }
    }
}
