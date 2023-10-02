using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public int Priority
    {
        get { return 100; }
    }

    public required string AgentExecutablePath { get; init; }
    public required IHistoryStopPointProcessingTask? Parent { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var historyStopPoint = Parent?.HistoryStopPoint;
            _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

            var logger = eventClient.ServiceProvider.GetService<ILogger<DetectManifestsUsingAgentActivity>>();
            logger?.LogDebug("Handling manifest detection for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId} with agent = {Agent}",
                historyStopPoint.CachedAnalysis.Id, historyStopPoint.Id, AgentExecutablePath);

            var cachedManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = await cachedManager.GetCacheDb();

            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath, cancellationToken);

            var manifestsFound = false;
            await foreach (var manifestPath in agentReader.DetectManifests(historyStopPoint.LocalPath).WithCancellation(cancellationToken))
            {
                manifestsFound = true;
                logger?.LogDebug(
                    "Detected manifest {ManifestPath} for HistoryStopPointId = {HistoryStopPointId}",
                    manifestPath, historyStopPoint.Id
                );

                var cachedManifest = await cacheDb.RetrieveManifest(historyStopPoint, manifestPath);
                if (cachedManifest != null)
                {
                    continue;
                }

                var manifest = await cacheDb.AddManifest(historyStopPoint, manifestPath);

                await eventClient.Fire(
                    new ManifestDetectedEvent
                    {
                        Parent = this,
                        AgentExecutablePath = AgentExecutablePath,
                        Manifest = manifest
                    },
                    cancellationToken
                );
            }

            if (!manifestsFound)
            {
                await eventClient.Fire(
                    new NoManifestsDetectedEvent { Parent = this },
                    cancellationToken
                );
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(this, error), cancellationToken);
        }
    }

    public override string ToString()
    {
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - AgentExecutablePath = {AgentExecutablePath}";
    }
}
