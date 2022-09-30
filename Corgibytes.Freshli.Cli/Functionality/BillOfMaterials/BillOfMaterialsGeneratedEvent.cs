using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : IApplicationEvent
{
    public BillOfMaterialsGeneratedEvent(IHistoryStopData historyStopData, string pathToBillOfMaterials)
    {
        HistoryStopData = historyStopData;
        PathToBillOfMaterials = pathToBillOfMaterials;
    }

    public IHistoryStopData HistoryStopData { get; }
    public string PathToBillOfMaterials { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new ComputeLibYearActivity(
            PathToBillOfMaterials,
            HistoryStopData
        ));
}
