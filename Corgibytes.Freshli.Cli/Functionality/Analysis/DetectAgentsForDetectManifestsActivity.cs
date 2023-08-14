using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }

    public int Priority
    {
        get { return 100; }
    }

    public DetectAgentsForDetectManifestsActivity(Guid analysisId, IHistoryStopPointProcessingTask parent)
    {
        AnalysisId = analysisId;
        Parent = parent;
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetService<ILogger<DetectAgentsForDetectManifestsActivity>>();
        logger?.LogTrace("Handling agents detection for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}", AnalysisId, Parent.HistoryStopPointId);

        try
        {
            var agentsDetector = eventClient.ServiceProvider.GetRequiredService<IAgentsDetector>();
            var agents = agentsDetector.Detect();

            if (agents.Count == 0)
            {
                await eventClient.Fire(
                    new NoAgentsDetectedFailureEvent { ErrorMessage = "Could not locate any agents" },
                    cancellationToken);
                return;
            }

            logger?.LogTrace("Found {Count} agents to detect manifests of AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}",
                agents.Count, AnalysisId, Parent.HistoryStopPointId);
            foreach (var agentPath in agents)
            {
                await eventClient.Fire(
                    new AgentDetectedForDetectManifestEvent(AnalysisId, Parent, agentPath),
                    cancellationToken);
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingFailedEvent(Parent, error),
                cancellationToken);
        }
    }
}
