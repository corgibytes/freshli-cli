using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

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

    public void Handle(IApplicationActivityEngine eventClient) => throw new NotImplementedException();
}
