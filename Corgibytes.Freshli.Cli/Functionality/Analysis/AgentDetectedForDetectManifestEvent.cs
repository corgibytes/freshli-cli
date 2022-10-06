using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : IApplicationEvent
{
    public AgentDetectedForDetectManifestEvent(Guid analysisId, IHistoryStopData historyStopData,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopData HistoryStopData { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new DetectManifestsUsingAgentActivity(AnalysisId, HistoryStopData, AgentExecutablePath));
}
