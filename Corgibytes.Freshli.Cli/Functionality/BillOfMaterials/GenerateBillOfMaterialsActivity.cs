using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity
{
    public readonly Guid AnalysisId;
    public readonly string AgentExecutablePath;
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly string ManifestPath;

    public GenerateBillOfMaterialsActivity(Guid analysisId, string agentExecutablePath,
        IAnalysisLocation analysisLocation,
        string manifestPath)
    {
        AnalysisId = analysisId;
        AgentExecutablePath = agentExecutablePath;
        AnalysisLocation = analysisLocation;
        ManifestPath = manifestPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var asOfDate = DateTime.Now;
        var pathToBillOfMaterials =
            agentReader.ProcessManifest(Path.Combine(AnalysisLocation.Path, ManifestPath), asOfDate);

        eventClient.Fire(new BillOfMaterialsGeneratedEvent(AnalysisId, AnalysisLocation, pathToBillOfMaterials));
    }
}
