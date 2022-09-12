using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity
{
    public readonly IAgentReader AgentReader;
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly string ManifestPath;

    public GenerateBillOfMaterialsActivity(IAgentReader agentReader, IAnalysisLocation analysisLocation, string manifestPath)
    {
        AgentReader = agentReader;
        AnalysisLocation = analysisLocation;
        ManifestPath = manifestPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var asOfDate = DateTime.Now;
        var pathToBillOfMaterials = AgentReader.ProcessManifest(ManifestPath, asOfDate);

        eventClient.Fire(new BillOfMaterialsGeneratedEvent(AnalysisLocation, pathToBillOfMaterials));
    }
}
