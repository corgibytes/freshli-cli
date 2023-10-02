using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity, ISynchronized, IHistoryStopPointProcessingTask
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_semaphores = new();

    public IHistoryStopPointProcessingTask? Parent => null;
    public required CachedHistoryStopPoint HistoryStopPoint { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetService<ILogger<CheckoutHistoryActivity>>();
        logger?.LogDebug("Handling history checkout for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}", HistoryStopPoint.CachedAnalysis.Id, HistoryStopPoint.Id);

        if (HistoryStopPoint.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        await gitManager.CreateArchive(
            HistoryStopPoint.RepositoryId,
            gitManager.ParseCommitId(HistoryStopPoint.GitCommitId)
        );

        logger?.LogDebug("Fire HistoryStopCheckedOutEvent for AnalysisId = {AnalysisId} and HistoryStopPointId = {HistoryStopPointId}", HistoryStopPoint.CachedAnalysis.Id, HistoryStopPoint.Id);
        await eventClient.Fire(
            new HistoryStopCheckedOutEvent
            {
                Parent = this
            },
            cancellationToken);

        new Thread(() =>
        {
            try
            {
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

    public SemaphoreSlim GetSemaphore()
    {
        if (HistoryStopPoint.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        return s_semaphores.GetOrAdd(HistoryStopPoint.GitCommitId, new SemaphoreSlim(1, 1));
    }

    public override string ToString()
    {
        var historyStopPointId = HistoryStopPoint.Id;

        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name}";
    }
}
