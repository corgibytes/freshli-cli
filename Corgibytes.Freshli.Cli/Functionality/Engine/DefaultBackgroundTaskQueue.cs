// Derived from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public sealed class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    private readonly object _statisticsLock = new();
    private QueueStatistics _statistics;

    public DefaultBackgroundTaskQueue()
    {
        _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();
    }

    public async ValueTask QueueBackgroundWorkItemAsync(
        Func<CancellationToken, ValueTask> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        IncrementEnqueuedCount();
        await _queue.Writer.WriteAsync(WrapWorkItem(workItem));
    }

    private Func<CancellationToken, ValueTask> WrapWorkItem(Func<CancellationToken, ValueTask> workItem)
    {
        return async (cancellationToken) =>
        {
            try
            {
                await workItem(cancellationToken);
                IncrementSucceededCount();
            }
            catch (OperationCanceledException)
            {
                // Operation canceled should not be counted as a failure
                throw;
            }
            catch (Exception)
            {
                IncrementFailedCount();
                throw;
            }
            finally
            {
                DecrementProcessingCount();
            }
        };
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        // this assumes that a work item is going to start processing after it has been removed from the queue
        IncrementProcessingCount();
        DecrementEnqueuedCount();

        return workItem;
    }

    public QueueStatistics GetStatistics()
    {
        lock (_statisticsLock)
        {
            return _statistics;
        }
    }

    private void IncrementEnqueuedCount()
    {
        lock (_statisticsLock)
        {
            _statistics = _statistics with { Enqueued = _statistics.Enqueued + 1 };
        }
    }

    private void DecrementEnqueuedCount()
    {
        lock (_statisticsLock)
        {
            _statistics = _statistics with { Enqueued = _statistics.Enqueued - 1 };
        }
    }

    private void IncrementProcessingCount()
    {
        lock (_statisticsLock)
        {
            _statistics = _statistics with { Processing = _statistics.Processing + 1 };
        }
    }

    private void DecrementProcessingCount()
    {
        lock (_statisticsLock)
        {
            _statistics = _statistics with { Processing = _statistics.Processing - 1 };
        }
    }

    private void IncrementFailedCount()
    {
        lock (_statisticsLock)
        {
            _statistics = _statistics with { Failed = _statistics.Failed + 1 };
        }
    }

    private void IncrementSucceededCount()
    {
        lock (_statisticsLock)
        {
            _statistics = _statistics with { Succeeded = _statistics.Succeeded + 1 };
        }
    }
}
