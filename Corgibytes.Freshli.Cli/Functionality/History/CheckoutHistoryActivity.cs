using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity, ISynchronized, IHistoryStopPointProcessingTask
{
    public CheckoutHistoryActivity(Guid analysisId, int historyStopPointId)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
    }

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_semaphores = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public Guid AnalysisId { get; }

    public IHistoryStopPointProcessingTask Parent => null!;
    public int HistoryStopPointId { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetService<ILogger<CheckoutHistoryActivity>>();
        logger?.LogDebug("Handling history checkout for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}", AnalysisId, HistoryStopPointId);

        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        if (historyStopPoint?.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }
        logger?.LogDebug("historyStopPoint = {@HistoryStopPoint} for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}",
            historyStopPoint, AnalysisId, HistoryStopPointId);

        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        await gitManager.CreateArchive(
            historyStopPoint.RepositoryId,
            gitManager.ParseCommitId(historyStopPoint.GitCommitId)
        );

        logger?.LogDebug("Fire HistoryStopCheckedOutEvent for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}", AnalysisId, HistoryStopPointId);
        await eventClient.Fire(
            new HistoryStopCheckedOutEvent
            {
                AnalysisId = AnalysisId,
                Parent = this
            },
            cancellationToken);

        new Thread(() =>
        {
            try {
                eventClient.Wait(this, cancellationToken).AsTask().Wait(cancellationToken);
                eventClient.Fire(
                    new HistoryStopPointProcessingCompletedEvent { Parent = this },
                    cancellationToken,
                    ApplicationTaskMode.Untracked
                ).AsTask().Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Don't do anything, we're just exiting
            }
            catch (Exception error)
            {
                eventClient.Fire(
                    new UnhandledExceptionEvent(error),
                    cancellationToken,
                    ApplicationTaskMode.Untracked
                ).AsTask().Wait(cancellationToken);
            }
        }).Start();

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
