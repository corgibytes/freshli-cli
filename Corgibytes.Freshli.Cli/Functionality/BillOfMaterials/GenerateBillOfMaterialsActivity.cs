using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity
{
    public readonly string AgentExecutablePath;
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly string ManifestPath;

    public GenerateBillOfMaterialsActivity(string agentExecutablePath, IAnalysisLocation analysisLocation, string manifestPath)
    {
        AgentExecutablePath = agentExecutablePath;
        AnalysisLocation = analysisLocation;
        ManifestPath = manifestPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var asOfDate = DateTime.Now;
        var pathToBillOfMaterials = agentReader.ProcessManifest(System.IO.Path.Combine(AnalysisLocation.Path, ManifestPath), asOfDate);

        eventClient.Fire(new BillOfMaterialsGeneratedEvent(AnalysisLocation, pathToBillOfMaterials));
    }
}
