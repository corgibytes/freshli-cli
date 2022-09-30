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
    public readonly IHistoryStopData HistoryStopData;
    public readonly string ManifestPath;

    public GenerateBillOfMaterialsActivity(string agentExecutablePath, IHistoryStopData historyStopData,
        string manifestPath)
    {
        AgentExecutablePath = agentExecutablePath;
        HistoryStopData = historyStopData;
        ManifestPath = manifestPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var asOfDate = DateTime.Now;
        var pathToBillOfMaterials =
            agentReader.ProcessManifest(Path.Combine(HistoryStopData.Path, ManifestPath), asOfDate);

        eventClient.Fire(new BillOfMaterialsGeneratedEvent(HistoryStopData, pathToBillOfMaterials));
    }
}
