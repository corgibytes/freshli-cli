using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity
{
    public DetectManifestsUsingAgentActivity(IAnalysisLocation analysisLocation, IAgentReader agentReader)
    {
        AnalysisLocation = analysisLocation;
        AgentReader = agentReader;
    }

    public IAnalysisLocation AnalysisLocation { get; }
    public IAgentReader AgentReader { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        foreach (var manifestPath in AgentReader.DetectManifests(AnalysisLocation.Path))
        {
            eventClient.Fire(new ManifestDetectedEvent(AnalysisLocation, AgentReader, manifestPath));
        }
    }
}
