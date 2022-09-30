using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : IApplicationEvent
{
    public ManifestDetectedEvent(IHistoryStopData historyStopData, string agentExecutablePath, string manifestPath)
    {
        HistoryStopData = historyStopData;
        AgentExecutablePath = agentExecutablePath;
        ManifestPath = manifestPath;
    }

    public IHistoryStopData HistoryStopData { get; }
    public string AgentExecutablePath { get; }
    public string ManifestPath { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(
        new GenerateBillOfMaterialsActivity(
            AgentExecutablePath,
            HistoryStopData,
            ManifestPath
        ));
}
