using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public BillOfMaterialsGeneratedEvent(Guid analysisId, int historyStopPointId,
        string pathToBillOfMaterials, string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        PathToBillOfMaterials = pathToBillOfMaterials;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string PathToBillOfMaterials { get; }
    public string AgentExecutablePath { get; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        try
        {
            await eventClient.Dispatch(new DeterminePackagesFromBomActivity(
                AnalysisId,
                HistoryStopPointId,
                PathToBillOfMaterials,
                AgentExecutablePath
            ));
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(new FireHistoryStopPointProcessingErrorActivity(HistoryStopPointId, error));
        }
    }
}
