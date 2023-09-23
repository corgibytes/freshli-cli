using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string AgentExecutablePath { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new DetectManifestsUsingAgentActivity
                {
                    Parent = this,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken
            );
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(this, error),
                cancellationToken);
        }
    }
}
