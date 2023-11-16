using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : ApplicationActivityBase, ISynchronized, IHistoryStopPointProcessingTask
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_semaphores = new();

    public IHistoryStopPointProcessingTask? Parent => null;
    public required CachedHistoryStopPoint HistoryStopPoint { get; init; }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetService<ILogger<CheckoutHistoryActivity>>();
        logger?.LogDebug("HistoryStopPoint = {HistoryStopPointId}: Handling history checkout", HistoryStopPoint.Id);

        if (HistoryStopPoint.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        new Thread(WaitForChildTasks).Start();

        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        logger?.LogTrace("HistoryStopPoint = {HistoryStopPointId}: Started checking out history", HistoryStopPoint.Id);
        await gitManager.CreateArchive(
            HistoryStopPoint.RepositoryId,
            gitManager.ParseCommitId(HistoryStopPoint.GitCommitId)
        );
        stopWatch.Stop();
        logger?.LogTrace("HistoryStopPoint = {HistoryStopPointId}: Stopped checking out history. Elapsed time - {Duration}", HistoryStopPoint.Id, stopWatch.Elapsed);

        await eventClient.Fire(
            new HistoryStopCheckedOutEvent
            {
                Parent = this
            },
            cancellationToken);

        return;

        async void WaitForChildTasks()
        {
            ApplicationTaskWaitToken? waitToken = null;
            try
            {
                logger?.LogTrace("HistoryStopPoint = {HistoryStopPointId}: Started waiting for all child tasks to complete", HistoryStopPoint.Id);
                var childWaitStopWatch = new Stopwatch();
                childWaitStopWatch.Start();
                waitToken = new ApplicationTaskWaitToken();
                await eventClient.RegisterChildWaitToken(this, waitToken, cancellationToken);
                await eventClient.Wait(this, cancellationToken, waitToken);
                childWaitStopWatch.Stop();
                logger?.LogTrace("HistoryStopPoint = {HistoryStopPointId}: Finished waiting for child tasks to complete. Elapsed time - {Duration}", HistoryStopPoint.Id, childWaitStopWatch.Elapsed);

                await eventClient.Fire(new HistoryStopPointProcessingCompletedEvent { Parent = this },
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Don't do anything, we're just exiting
            }
            catch (Exception error)
            {
                await eventClient.Fire(new UnhandledExceptionEvent(error), cancellationToken,
                    ApplicationTaskMode.Untracked);
            }
            finally
            {
                waitToken?.Signal();
            }
        }
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
