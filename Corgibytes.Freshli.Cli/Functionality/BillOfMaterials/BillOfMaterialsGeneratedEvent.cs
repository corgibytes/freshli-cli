using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : IApplicationEvent
{
    public BillOfMaterialsGeneratedEvent(Guid analysisId, IAnalysisLocation analysisLocation,
        string pathToBillOfMaterials)
    {
        AnalysisId = analysisId;
        AnalysisLocation = analysisLocation;
        PathToBillOfMaterials = pathToBillOfMaterials;
    }

    public Guid AnalysisId { get; }
    public IAnalysisLocation AnalysisLocation { get; }
    public string PathToBillOfMaterials { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new ComputeLibYearActivity(
            AnalysisId,
            PathToBillOfMaterials,
            AnalysisLocation
        ));
}
