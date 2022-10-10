using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : IApplicationEvent
{
    public BillOfMaterialsGeneratedEvent(Guid analysisId, IHistoryStopData historyStopData,
        string pathToBillOfMaterials, string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
        PathToBillOfMaterials = pathToBillOfMaterials;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopData HistoryStopData { get; }
    public string PathToBillOfMaterials { get; }
    public string AgentExecutablePath { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new ComputeLibYearForBomActivity(
            AnalysisId,
            HistoryStopData,
            PathToBillOfMaterials,
            AgentExecutablePath
        ));
}
