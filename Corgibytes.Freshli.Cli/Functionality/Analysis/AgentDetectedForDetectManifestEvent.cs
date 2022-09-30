using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : IApplicationEvent
{
    public AgentDetectedForDetectManifestEvent(IHistoryStopData historyStopData, string agentExecutablePath)
    {
        HistoryStopData = historyStopData;
        AgentExecutablePath = agentExecutablePath;
    }

    public IHistoryStopData HistoryStopData { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new DetectManifestsUsingAgentActivity(HistoryStopData, AgentExecutablePath));
}
