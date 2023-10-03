using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public record ApplicationTaskWaitToken(ManualResetEventSlim ResetEvent, ConcurrentBag<ApplicationTaskWaitToken> ChildWaitInfos)
{
    private readonly ConcurrentDictionary<Task, bool> _waitTasksAndAddedStatus = new();

    public ApplicationTaskWaitToken() : this(new ManualResetEventSlim(false), new ConcurrentBag<ApplicationTaskWaitToken>())
    {
    }

    public void Signal()
    {
        ResetEvent.Set();
    }

    public void AddChildResetEvent(ApplicationTaskWaitToken childApplicationTaskWaitToken)
    {
        ChildWaitInfos.Add(childApplicationTaskWaitToken);

        while (!_waitTasksAndAddedStatus.Values.All(value => value))
        {
            foreach (var task in _waitTasksAndAddedStatus.Keys)
            {
                _waitTasksAndAddedStatus.TryUpdate(task, true, false);
            }
        }
    }

    private enum WaitMode
    {
        All,
        ChildrenOnly
    }

    private async Task<bool> Wait(ApplicationTaskWaitToken? excluding, CancellationToken cancellationToken,
        WaitMode waitMode = WaitMode.All)
    {
        var childWaitTask = Task.Run(
            async () =>
            {
                await Task.WhenAll(
                    ChildWaitInfos.ToArray().Where(item => item != excluding).Select(item => item.Wait(cancellationToken))
                );
            },
            cancellationToken
        );

        var childAddedDuringWait = false;
        _waitTasksAndAddedStatus.TryAdd(childWaitTask, childAddedDuringWait);

        if (waitMode == WaitMode.All)
        {
            ResetEvent.Wait(cancellationToken);
        }

        await childWaitTask;

        _waitTasksAndAddedStatus.TryRemove(childWaitTask, out childAddedDuringWait);

        while (childAddedDuringWait)
        {
            childAddedDuringWait = await Wait(excluding, cancellationToken, WaitMode.ChildrenOnly);
        }

        return childAddedDuringWait;
    }

    public async Task Wait(CancellationToken cancellationToken, ApplicationTaskWaitToken? excluding = null)
    {
        await Wait(excluding, cancellationToken);
    }
}
