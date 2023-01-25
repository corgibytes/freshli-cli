// Derived from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    ValueTask<WorkItem> DequeueAsync(CancellationToken cancellationToken = default);
    ValueTask<bool> ContainsUnprocessedWork<T>(Func<T, bool> query, CancellationToken cancellationToken = default);
}
