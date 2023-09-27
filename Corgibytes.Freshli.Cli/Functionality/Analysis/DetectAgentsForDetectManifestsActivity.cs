using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }

    public int Priority
    {
        get { return 100; }
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var historyStopPoint = Parent?.HistoryStopPoint;
            _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

            var logger = eventClient.ServiceProvider.GetService<ILogger<DetectAgentsForDetectManifestsActivity>>();
            logger?.LogTrace("Handling agents detection for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}", historyStopPoint.CachedAnalysis.Id, historyStopPoint.Id);

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
                agents.Count, historyStopPoint.CachedAnalysis.Id, historyStopPoint.Id);
            foreach (var agentPath in agents)
            {
                await eventClient.Fire(
                    new AgentDetectedForDetectManifestEvent { Parent = this, AgentExecutablePath = agentPath },
                    cancellationToken);
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingFailedEvent(this, error),
                cancellationToken);
        }
    }
}
