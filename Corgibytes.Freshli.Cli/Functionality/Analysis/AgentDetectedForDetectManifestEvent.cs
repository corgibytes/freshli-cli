using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : IApplicationEvent
{
    public AgentDetectedForDetectManifestEvent(IAnalysisLocation analysisLocation, string agentExecutablePath)
    {
        AnalysisLocation = analysisLocation;
        AgentExecutablePath = agentExecutablePath;
    }

    public IAnalysisLocation AnalysisLocation { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new DetectManifestsUsingAgentActivity(AnalysisLocation, AgentExecutablePath));
}
