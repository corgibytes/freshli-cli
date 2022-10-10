using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : IApplicationEvent
{
    public BillOfMaterialsGeneratedEvent(Guid analysisId, IHistoryStopData historyStopData,
        string pathToBillOfMaterials)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
        PathToBillOfMaterials = pathToBillOfMaterials;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopData HistoryStopData { get; }
    public string PathToBillOfMaterials { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new ComputeLibYearForBomActivity(
            AnalysisId,
            HistoryStopData,
            PathToBillOfMaterials
        ));
}
