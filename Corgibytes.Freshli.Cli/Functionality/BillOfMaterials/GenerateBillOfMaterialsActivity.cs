using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity
{
    public readonly string AgentExecutablePath;
    public readonly Guid AnalysisId;
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly string ManifestPath;

<<<<<<< HEAD
    public GenerateBillOfMaterialsActivity(IAgentReader agentReader, IAnalysisLocation analysisLocation,
=======
    public GenerateBillOfMaterialsActivity(Guid analysisId, string agentExecutablePath,
        IAnalysisLocation analysisLocation,
>>>>>>> 17d03008d745c9ae27235640a7d65e25e5fdf50f
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
