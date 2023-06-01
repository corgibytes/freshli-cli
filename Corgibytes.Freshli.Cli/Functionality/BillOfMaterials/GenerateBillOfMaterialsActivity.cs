using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity, ISynchronized, IHistoryStopPointProcessingTask
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_historyPointSemaphores = new();
    public readonly string AgentExecutablePath;
    public readonly Guid AnalysisId;
    public readonly string ManifestPath;
    public IHistoryStopPointProcessingTask Parent { get; }

    public int Priority
    {
        get { return 100; }
    }

    public GenerateBillOfMaterialsActivity(Guid analysisId, string agentExecutablePath,
        IHistoryStopPointProcessingTask parent, string manifestPath)
    {
        AnalysisId = analysisId;
        AgentExecutablePath = agentExecutablePath;
        Parent = parent;
        ManifestPath = manifestPath;
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();

            var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(Parent.HistoryStopPointId);
            _ = historyStopPoint ?? throw new Exception($"Failed to retrieve history stop point {Parent.HistoryStopPointId}");

            var historyPointPath = historyStopPoint.LocalPath;
            var asOfDateTime = historyStopPoint.AsOfDateTime;

            var fullManifestPath = Path.Combine(historyPointPath, ManifestPath);
            var bomFilePath = await agentReader.ProcessManifest(fullManifestPath, asOfDateTime);
            var cachedBomFilePath = await cacheManager.StoreBomInCache(bomFilePath, AnalysisId, asOfDateTime);

            await eventClient.Fire(
                new BillOfMaterialsGeneratedEvent(
                    AnalysisId,
                    Parent,
                    cachedBomFilePath,
                    AgentExecutablePath
                ),
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingFailedEvent(Parent, error),
                cancellationToken);
        }
    }

    public async ValueTask<SemaphoreSlim> GetSemaphore(IServiceProvider provider)
    {
        var cacheManager = provider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(Parent.HistoryStopPointId);
        // TODO create an exception class for this exception and write a test to cover it getting generated
        _ = historyStopPoint ?? throw new Exception($"Failed to retrieve history stop point {Parent.HistoryStopPointId}");

        var historyPointPath = historyStopPoint.LocalPath;

        return s_historyPointSemaphores.GetOrAdd(historyPointPath, new SemaphoreSlim(1, 1));
    }
}
