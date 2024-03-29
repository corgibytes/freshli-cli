// Derived from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class BackgroundTaskQueue : IBackgroundTaskQueue, IDisposable
{
    private readonly SemaphoreSlim _queueReaderSemaphore = new(0);
    private readonly PriorityQueue<WorkItem, int> _queue = new();
    private readonly SemaphoreSlim _pendingWorkItemsSemaphore = new(1);
    private readonly List<WorkItem> _pendingWorkItems = new();

    private readonly SemaphoreSlim _statisticsSemaphore = new(1);
    private QueueStatistics _statistics;

    public async ValueTask QueueBackgroundWorkItemAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        _ = workItem.ApplicationTask ?? throw new ArgumentNullException(nameof(workItem), CliOutput.BackgroundTaskQueue_QueueBackgroundWorkItemAsync_workItem_Argument_cannot_be_null);
        _ = workItem.Invoker ?? throw new ArgumentNullException(nameof(workItem), CliOutput.BackgroundTaskQueue_QueueBackgroundWorkItemAsync_workItem_Invoker_cannot_be_null);
        await IncrementEnqueuedCount(cancellationToken);

        await AddToPendingWorkItems(workItem, cancellationToken);

        lock (_queue)
        {
            _queue.Enqueue(WrapWorkItem(workItem), workItem.ApplicationTask.Priority);
        }

        _queueReaderSemaphore.Release();
    }

    private async ValueTask AddToPendingWorkItems(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        await WithSemaphoreLock(() =>
        {
            _pendingWorkItems.Add(workItem);
        }, _pendingWorkItemsSemaphore, cancellationToken);
    }

    private async ValueTask RemoveFromPendingWorkItems(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        await WithSemaphoreLock(() =>
        {
            _pendingWorkItems.Remove(workItem);
        }, _pendingWorkItemsSemaphore, cancellationToken);
    }

    private WorkItem WrapWorkItem(WorkItem workItem)
    {
        return new WorkItem(workItem.ApplicationTask, async cancellationToken =>
        {
            try
            {
                await workItem.Invoker(cancellationToken);
                await IncrementSucceededCount(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Operation canceled should not be counted as a failure
                throw;
            }
            catch (Exception)
            {
                await IncrementFailedCount(cancellationToken);
                throw;
            }
            finally
            {
                await RemoveFromPendingWorkItems(workItem, cancellationToken);
                await DecrementProcessingCount(cancellationToken);
            }
        });
    }

    public async ValueTask<WorkItem> DequeueAsync(CancellationToken cancellationToken)
    {
        await _queueReaderSemaphore.WaitAsync(cancellationToken).ConfigureAwait(true);
        bool dequeueResult;
        WorkItem workItem;

        lock (_queue)
        {
            dequeueResult = _queue.TryDequeue(out workItem, out var _);
        }

        if (!dequeueResult)
        {
            throw new InvalidOperationException("Failed to retrieve an item from the queue");
        }

        if (workItem.ApplicationTask == null && workItem.Invoker == null)
        {
            throw new InvalidOperationException("workItem ApplicationTask and Invoker are both null");
        }

        if (workItem.ApplicationTask == null)
        {
            throw new InvalidOperationException("workItem ApplicationTask is null");
        }

        if (workItem.Invoker == null)
        {
            throw new InvalidOperationException("workItem Invoker is null");
        }

        // this assumes that a work item is going to start processing after it has been removed from the queue
        await IncrementProcessingCount(cancellationToken);
        await DecrementEnqueuedCount(cancellationToken);

        return workItem;
    }

    public QueueStatistics GetStatistics()
    {
        return _statistics;
    }

    public async ValueTask<bool> ContainsUnprocessedWork<T>(Func<T, bool> query, CancellationToken cancellationToken = default)
    {
        var result = true;

        await WithSemaphoreLock(() =>
        {
            result = _pendingWorkItems.Any(item => item.ApplicationTask is T task && query(task));
        }, _pendingWorkItemsSemaphore, cancellationToken);

        return result;
    }

    private async ValueTask IncrementEnqueuedCount(CancellationToken cancellationToken = default)
    {
        await WithStatisticsLock(() =>
        {
            _statistics = _statistics with { Enqueued = _statistics.Enqueued + 1 };
        }, cancellationToken);
    }

    private async ValueTask DecrementEnqueuedCount(CancellationToken cancellationToken = default)
    {
        await WithStatisticsLock(() =>
        {
            _statistics = _statistics with { Enqueued = _statistics.Enqueued - 1 };
        }, cancellationToken);
    }

    private async ValueTask IncrementProcessingCount(CancellationToken cancellationToken = default)
    {
        await WithStatisticsLock(() =>
        {
            _statistics = _statistics with { Processing = _statistics.Processing + 1 };
        }, cancellationToken);
    }

    private async ValueTask DecrementProcessingCount(CancellationToken cancellationToken = default)
    {
        await WithStatisticsLock(() =>
        {
            _statistics = _statistics with { Processing = _statistics.Processing - 1 };
        }, cancellationToken);
    }

    private async ValueTask IncrementFailedCount(CancellationToken cancellationToken = default)
    {
        await WithStatisticsLock(() =>
        {
            _statistics = _statistics with { Failed = _statistics.Failed + 1 };
        }, cancellationToken);
    }

    private async ValueTask IncrementSucceededCount(CancellationToken cancellationToken = default)
    {
        await WithStatisticsLock(() =>
        {
            _statistics = _statistics with { Succeeded = _statistics.Succeeded + 1 };
        }, cancellationToken);
    }

    private async ValueTask WithStatisticsLock(Action operation, CancellationToken cancellationToken = default)
    {
        await WithSemaphoreLock(operation, _statisticsSemaphore, cancellationToken);
    }

    private static async ValueTask WithSemaphoreLock(Action operation, SemaphoreSlim semaphore,
        CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(true);
        try
        {
            operation();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void Dispose()
    {
        _queueReaderSemaphore.Dispose();
        _pendingWorkItemsSemaphore.Dispose();
        _statisticsSemaphore.Dispose();

        GC.SuppressFinalize(this);
    }
}
