using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity, ISynchronized, IHistoryStopPointProcessingTask
{
    public int Priority
    {
        get { return 100; }
    }

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_historyPointSemaphores = new();

    public required string AgentExecutablePath { get; init; }
    public required IHistoryStopPointProcessingTask? Parent { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var historyStopPoint = Parent?.HistoryStopPoint;
            _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

            var manifest = Parent?.Manifest;
            _ = manifest ?? throw new Exception("Parent's Manifest is null");

            var logger = eventClient.ServiceProvider.GetService<ILogger<GenerateBillOfMaterialsActivity>>();
            logger?.LogDebug("Handling bom generation for HistoryStopPointId = {HistoryStopPointId} with agent = {Agent} and manifest path = {Path}",
                historyStopPoint.Id, AgentExecutablePath, manifest.ManifestFilePath);

            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath, cancellationToken);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();

            var historyPointPath = historyStopPoint.LocalPath;
            var asOfDateTime = historyStopPoint.AsOfDateTime;

            logger?.LogDebug("Preparing to process manifest for HistoryStopPointId = {HistoryStopPointId} with agent = {Agent} for {Path} on {AsOfDate}",
                historyStopPoint.Id, AgentExecutablePath, manifest.ManifestFilePath, asOfDateTime);

            var fileValidator = eventClient.ServiceProvider.GetRequiredService<IFileValidator>();

            string bomFilePath;
            try
            {
                bomFilePath = await agentReader.ProcessManifest(manifest.ManifestFilePath, asOfDateTime);
                logger?.LogDebug("BillOfMaterials is {BomFilePath} generated from {FullManifestPath}",
                    bomFilePath, manifest.ManifestFilePath);

                if (!fileValidator.IsValidFilePath(bomFilePath))
                {
                    logger?.LogWarning("Processing manifest {ManifestPath} failed to generate BOM file for {AsOfDate}",
                        manifest.ManifestFilePath, asOfDateTime);
                    return;
                }
            }
            catch (Exception e)
            {
                logger?.LogWarning("Exception attempting to process manifest {ManifestPath} for {AsOfDate}: {Message}",
                    manifest.ManifestFilePath, asOfDateTime, e.Message);
                return;
            }

            var cachedBomFilePath = await cacheManager.StoreBomInCache(bomFilePath, historyStopPoint.CachedAnalysis.Id, asOfDateTime, manifest.ManifestFilePath);

            await eventClient.Fire(
                new BillOfMaterialsGeneratedEvent
                {
                    Parent = this,
                    PathToBillOfMaterials = cachedBomFilePath,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken
            );
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingFailedEvent(this, error),
                cancellationToken
            );
        }
    }

    public SemaphoreSlim GetSemaphore()
    {
        var historyStopPoint = Parent?.HistoryStopPoint;
        // TODO create an exception class for this exception and write a test to cover it getting generated
        _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

        var historyPointPath = historyStopPoint.LocalPath;

        return s_historyPointSemaphores.GetOrAdd(historyPointPath, new SemaphoreSlim(1, 1));
    }
}
