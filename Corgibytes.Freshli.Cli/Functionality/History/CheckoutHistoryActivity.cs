using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity, ISynchronized
{
    public CheckoutHistoryActivity(Guid analysisId, int historyStopPointId)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
    }

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_semaphores = new();

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public Guid AnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public int HistoryStopPointId { get; set; }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        if (historyStopPoint?.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        await gitManager.CreateArchive(
            historyStopPoint.RepositoryId,
            gitManager.ParseCommitId(historyStopPoint.GitCommitId)
        );

        await eventClient.Fire(new HistoryStopCheckedOutEvent
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId
        });
    }

    public async ValueTask<SemaphoreSlim> GetSemaphore(IServiceProvider serviceProvider)
    {
        var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        if (historyStopPoint?.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        return s_semaphores.GetOrAdd(historyStopPoint.GitCommitId, new SemaphoreSlim(1, 1));
    }
}
