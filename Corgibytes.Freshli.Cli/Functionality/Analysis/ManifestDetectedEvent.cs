using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : IApplicationEvent
{
    public ManifestDetectedEvent(IAnalysisLocation analysisLocation, string agentExecutablePath, string manifestPath)
    {
        AnalysisLocation = analysisLocation;
        AgentExecutablePath = agentExecutablePath;
        ManifestPath = manifestPath;
    }

    public IAnalysisLocation AnalysisLocation { get; }
    public string AgentExecutablePath { get; }
    public string ManifestPath { get; }

    public void Handle(IApplicationActivityEngine eventClient) => eventClient.Dispatch(new GenerateBillOfMaterialsActivity(
        AgentExecutablePath,
        AnalysisLocation,
        ManifestPath
    ));
}
