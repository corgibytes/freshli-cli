using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : IApplicationEvent
{
    public IAnalysisLocation AnalysisLocation { get; }
    public IAgentReader AgentReader { get; }
    public string ManifestPath { get; }

    public ManifestDetectedEvent(IAnalysisLocation analysisLocation, IAgentReader agentReader, string manifestPath)
    {
        AnalysisLocation = analysisLocation;
        AgentReader = agentReader;
        ManifestPath = manifestPath;
    }

    public void Handle(IApplicationActivityEngine eventClient) => throw new System.NotImplementedException();
}
