using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public AgentDetectedForDetectManifestEvent(Guid analysisId, IHistoryStopPointProcessingTask parent,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        Parent = parent;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }
    public string AgentExecutablePath { get; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new DetectManifestsUsingAgentActivity(AnalysisId, Parent, AgentExecutablePath),
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(Parent, error),
                cancellationToken);
        }
    }
}
