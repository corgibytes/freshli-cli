using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : IApplicationEvent
{
    public AgentDetectedForDetectManifestEvent(IAnalysisLocation analysisLocation, IAgentReader agentReader)
    {
        AnalysisLocation = analysisLocation;
        AgentReader = agentReader;
    }

    public IAnalysisLocation AnalysisLocation { get; }
    public IAgentReader AgentReader { get; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new DetectManifestsUsingAgentActivity(AnalysisLocation, AgentReader));
}
