using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : IApplicationEvent
{
    public BillOfMaterialsGeneratedEvent(IAnalysisLocation analysisLocation, string pathToBillOfMaterials)
    {
        AnalysisLocation = analysisLocation;
        PathToBillOfMaterials = pathToBillOfMaterials;
    }

    public IAnalysisLocation AnalysisLocation { get; }
    public string PathToBillOfMaterials { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new ComputeLibYearActivity(
            PathToBillOfMaterials,
            AnalysisLocation
        ));
}
