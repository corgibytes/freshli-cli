using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public AgentDetectedForDetectManifestEvent(Guid analysisId, int historyStopPointId,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string AgentExecutablePath { get; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(
            new DetectManifestsUsingAgentActivity(AnalysisId, HistoryStopPointId, AgentExecutablePath));
}
