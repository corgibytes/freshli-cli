using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : IApplicationEvent
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

    public async ValueTask Handle(IApplicationActivityEngine eventClient) => await eventClient.Dispatch(
        new DeterminePackagesFromBomActivity(
            AnalysisId,
            HistoryStopPointId,
            PathToBillOfMaterials,
            AgentExecutablePath
        ));
}
