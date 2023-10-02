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

    public async Task Wait(CancellationToken cancellationToken, ApplicationTaskWaitToken? excluding = null)
    {
        var childWaitTask = Task.WhenAll(
            ChildWaitInfos.ToArray().Where(item => item != excluding).Select(item => item.Wait(cancellationToken))
        );

        var childAddedDuringWait = false;
        _waitTasksAndAddedStatus.TryAdd(childWaitTask, childAddedDuringWait);

        ResetEvent.Wait(cancellationToken);
        await childWaitTask;

        _waitTasksAndAddedStatus.TryRemove(childWaitTask, out childAddedDuringWait);

        while (childAddedDuringWait)
        {
            childWaitTask = Task.WhenAll(
                ChildWaitInfos.ToArray().Where(item => item != excluding).Select(item => item.Wait(cancellationToken))
            );
            _waitTasksAndAddedStatus.TryAdd(childWaitTask, false);

            await childWaitTask;

            _waitTasksAndAddedStatus.TryRemove(childWaitTask, out childAddedDuringWait);
        }
    }
}
