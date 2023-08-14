using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Engine;

[UnitTest]
public class BackgroundTaskQueueTest
{
    private class DefaultApplicationTask : IApplicationTask
    {
    }

    private class HighPriorityApplicationTask : IApplicationTask
    {
        public int Priority
        {
            get { return 0; }
        }
    }

    private readonly BackgroundTaskQueue _queue = new();

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task EmptyQueueContainsNoUnprocessedWork()
    {
        Assert.False(await _queue.ContainsUnprocessedWork<IApplicationTask>(_ => true));
    }

    [Fact]
    public void EmptyQueueContainsEmptyStatistics()
    {
        var statistics = _queue.GetStatistics();

        AssertQueueStatistics(enqueued: 0, processing: 0, failed: 0, succeeded: 0, statistics);
    }

    private static void AssertQueueStatistics(int enqueued, int processing, int failed, int succeeded, QueueStatistics statistics)
    {
        Assert.Equal(enqueued, statistics.Enqueued);
        Assert.Equal(processing, statistics.Processing);
        Assert.Equal(failed, statistics.Failed);
        Assert.Equal(succeeded, statistics.Succeeded);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task QueuedTasksGetAddedToUnprocessedWork()
    {
        await AddSingleItemToQueue<DefaultApplicationTask>();

        Assert.True(await _queue.ContainsUnprocessedWork<DefaultApplicationTask>(_ => true));
    }

    private async Task<Tuple<IApplicationTask, WorkItem>> AddSingleItemToQueue<TTask>(Func<CancellationToken, ValueTask>? work = default) where TTask : IApplicationTask, new()
    {
        work ??= _ => ValueTask.CompletedTask;
        var applicationTask = new TTask();
        var workItem = new WorkItem(applicationTask, work);
        await _queue.QueueBackgroundWorkItemAsync(workItem);

        return new Tuple<IApplicationTask, WorkItem>(applicationTask, workItem);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task QueuedTasksGetAddedToStatistics()
    {
        await AddSingleItemToQueue<DefaultApplicationTask>();

        var statistics = _queue.GetStatistics();

        AssertQueueStatistics(enqueued: 1, processing: 0, failed: 0, succeeded: 0, statistics);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task DequeuedTasksRemainInUnprocessedWork()
    {
        var (expectedTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();

        var actualItem = await _queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(expectedTask, actualItem.ApplicationTask);

        Assert.True(await _queue.ContainsUnprocessedWork<DefaultApplicationTask>(_ => true));
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task DequeueTasksShowUpAsInProgress()
    {
        var (expectedTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();

        var actualItem = await _queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(expectedTask, actualItem.ApplicationTask);

        var statistics = _queue.GetStatistics();
        AssertQueueStatistics(enqueued: 0, processing: 1, failed: 0, succeeded: 0, statistics);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CompletedTasksRemovedFromUnprocessedWork()
    {
        var (expectedTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();

        var actualItem = await _queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(expectedTask, actualItem.ApplicationTask);

        await actualItem.Invoker(CancellationToken.None);

        Assert.False(await _queue.ContainsUnprocessedWork<DefaultApplicationTask>(_ => true));
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CompletedTasksShowUpInStatistics()
    {
        var (expectedTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();

        var actualItem = await _queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(expectedTask, actualItem.ApplicationTask);

        await actualItem.Invoker(CancellationToken.None);

        var statistics = _queue.GetStatistics();
        AssertQueueStatistics(enqueued: 0, processing: 0, failed: 0, succeeded: 1, statistics);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FailedTasksRemovedFromUnprocessedWork()
    {
        var expectedException = new Exception();
        var (expectedTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>(_ => throw expectedException);

        var actualItem = await _queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(expectedTask, actualItem.ApplicationTask);

        try
        {
            await actualItem.Invoker(CancellationToken.None);
        }
        catch (Exception actualException)
        {
            Assert.Same(expectedException, actualException);
        }

        Assert.False(await _queue.ContainsUnprocessedWork<DefaultApplicationTask>(_ => true));
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FailedTasksShowUpInStatistics()
    {
        var expectedException = new Exception();
        var (expectedTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>(_ => throw expectedException);

        var actualItem = await _queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(expectedTask, actualItem.ApplicationTask);

        try
        {
            await actualItem.Invoker(CancellationToken.None);
        }
        catch (Exception actualException)
        {
            Assert.Same(expectedException, actualException);
        }

        var statistics = _queue.GetStatistics();
        AssertQueueStatistics(enqueued: 0, processing: 0, failed: 1, succeeded: 0, statistics);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task TasksWithTheSamePriorityReturnedInUndefinedOrder()
    {
        var (expectedFirstTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();
        var (expectedSecondTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();

        var firstItem = await _queue.DequeueAsync(CancellationToken.None);
        var secondItem = await _queue.DequeueAsync(CancellationToken.None);

        // Note: the `PriorityQueue<>` class that's being used by the `BackgroundTaskQueue` does not
        // return items of the same priority in a deterministic way. For more information see
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2?view=net-7.0.
        var applicationTasks = new List<IApplicationTask>
        {
            firstItem.ApplicationTask,
            secondItem.ApplicationTask
        };
        Assert.Contains(expectedFirstTask, applicationTasks);
        Assert.Contains(expectedSecondTask, applicationTasks);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task TasksWithDifferentPriorityReturnedInCorrectOrder()
    {
        var (expectedFirstDefaultTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();
        var (expectedFirstHighPriorityTask, _) = await AddSingleItemToQueue<HighPriorityApplicationTask>();
        var (expectedSecondDefaultTask, _) = await AddSingleItemToQueue<DefaultApplicationTask>();
        var (expectedSecondHighPriorityTask, _) = await AddSingleItemToQueue<HighPriorityApplicationTask>();

        var firstItem = await _queue.DequeueAsync(CancellationToken.None);
        var secondItem = await _queue.DequeueAsync(CancellationToken.None);
        var thirdItem = await _queue.DequeueAsync(CancellationToken.None);
        var fourthItem = await _queue.DequeueAsync(CancellationToken.None);

        // Note: the `PriorityQueue<>` class that's being used by the `BackgroundTaskQueue` does not
        // return items of the same priority in a deterministic way. For more information see
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2?view=net-7.0.
        var highPriorityTasks = new List<IApplicationTask>
        {
            firstItem.ApplicationTask,
            secondItem.ApplicationTask
        };

        var defaultPriorityTasks = new List<IApplicationTask>
        {
            thirdItem.ApplicationTask,
            fourthItem.ApplicationTask
        };

        Assert.Contains(expectedFirstHighPriorityTask, highPriorityTasks);
        Assert.Contains(expectedSecondHighPriorityTask, highPriorityTasks);
        Assert.Contains(expectedFirstDefaultTask, defaultPriorityTasks);
        Assert.Contains(expectedSecondDefaultTask, defaultPriorityTasks);
    }
}
