using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : IApplicationEvent
{
    public AgentDetectedForDetectManifestEvent(Guid analysisId, IAnalysisLocation analysisLocation,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        AnalysisLocation = analysisLocation;
        AgentExecutablePath = agentExecutablePath;
    }

    [JsonProperty] private Guid AnalysisId { get; }

    public IAnalysisLocation AnalysisLocation { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new DetectManifestsUsingAgentActivity(AnalysisId, AnalysisLocation, AgentExecutablePath));
}
