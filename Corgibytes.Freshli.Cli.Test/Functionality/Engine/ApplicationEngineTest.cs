using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Support;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NLog.Extensions.Hosting;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Support.Environment;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Engine;

[IntegrationTest]
public class ApplicationEngineTest : IDisposable
{
    private readonly ApplicationEngine _engine;

    private readonly QueuedHostedService _queuedHostedService;
    private readonly Task _workerTask;
    private readonly CancellationTokenSource _workerCancellation;
    private readonly IHost _host;

    private const int TreeFanOut = 10;
    private const int HandleDelayInMilliseconds = 100;

    private static readonly TimeSpan s_handleDelay = TimeSpan.FromMilliseconds(HandleDelayInMilliseconds);

    public ApplicationEngineTest()
    {
        var configuration = new Configuration(new Environment());
        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(NullLoggerProvider.Instance);
            })
            .UseNLog()
            .ConfigureServices((_, services) =>
            {
                new ServiceBuilder(services, configuration).Register();
            })
            .Build();

        _workerCancellation = new CancellationTokenSource();
        _queuedHostedService = ActivatorUtilities.CreateInstance<QueuedHostedService>(_host.Services);
        _workerTask = _queuedHostedService.StartAsync(_workerCancellation.Token);

        _engine = _host.Services.GetRequiredService<ApplicationEngine>();
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForTaskCompletion()
    {
        var activity = new FakeApplicationActivity();
        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task ActivityHandleMethodsAreOnlyCalledOnce()
    {
        var activity = new FakeApplicationActivityThatCountsCalls();
        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.Equal(1, activity.HandleCallCount);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task EventsAreOnlyFiredOnce()
    {
        var activity = new FakeApplicationActivityThatFiresAnEventThatCountsCalls();
        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.Equal(1, activity.ChildEvent.HandleCallCount);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForTaskCompletionWhenTaskHasChildren()
    {
        var activity = new FakeApplicationActivityThatFiresAnEvent();
        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForTaskCompletionWithTreeOfActivitiesAndEvents()
    {
        var activity = new FakeApplicationActivityTree();
        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        AssertHandled(activity);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForTaskCompletionWithTreeOfActivitiesAndEventsWithMultipleWorkers()
    {
        var workers = new List<(QueuedHostedService, Task, CancellationTokenSource)>();

        for (var index = 0; index < 5; index++)
        {
            var additionalWorkerCancellation = new CancellationTokenSource();
            var additionalQueuedHostedService = ActivatorUtilities.CreateInstance<QueuedHostedService>(_host.Services);
            var additionalWorkerTask = additionalQueuedHostedService.StartAsync(_workerCancellation.Token);

            workers.Add((additionalQueuedHostedService, additionalWorkerTask, additionalWorkerCancellation));
        }

        var activity = new FakeApplicationActivityTree();

        try
        {
            await _engine.Dispatch(activity, CancellationToken.None);
            await _engine.Wait(activity, CancellationToken.None);
        }
        finally
        {
            foreach (var (additionalQueuedHostedService, additionalWorkerTask, additionalWorkerCancellation) in workers)
            {
                additionalWorkerCancellation.Cancel();
                await additionalWorkerTask;

                additionalWorkerCancellation.Dispose();
                additionalWorkerTask.Dispose();
                additionalQueuedHostedService.Dispose();
            }
        }

        AssertHandled(activity);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForRegisteredApplicationTaskWaitToken()
    {
        var activity = new FakeApplicationActivityTreeThatWaitsOnChildren();

        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);
        var waitFinishedAt = DateTimeOffset.Now;

        AssertHandledBefore(activity, waitFinishedAt);
    }

    [Fact(/*Timeout = Constants.ExpandedTestTimeout*/)]
    public async Task WaitForRegisteredApplicationTaskWaitTokenWithMultipleWorkers()
    {
        var workers = new List<(QueuedHostedService, Task, CancellationTokenSource)>();

        for (var index = 0; index < 5; index++)
        {
            var additionalWorkerCancellation = new CancellationTokenSource();
            var additionalQueuedHostedService = ActivatorUtilities.CreateInstance<QueuedHostedService>(_host.Services);
            var additionalWorkerTask = additionalQueuedHostedService.StartAsync(_workerCancellation.Token);

            workers.Add((additionalQueuedHostedService, additionalWorkerTask, additionalWorkerCancellation));
        }

        var activity = new FakeApplicationActivityTreeThatWaitsOnChildren();

        DateTimeOffset waitFinishedAt;
        try
        {
            await _engine.Dispatch(activity, CancellationToken.None);
            await _engine.Wait(activity, CancellationToken.None);
            waitFinishedAt = DateTimeOffset.Now;
        }
        finally
        {
            foreach (var (additionalQueuedHostedService, additionalWorkerTask, additionalWorkerCancellation) in workers)
            {
                additionalWorkerCancellation.Cancel();
                await additionalWorkerTask;

                additionalWorkerCancellation.Dispose();
                additionalWorkerTask.Dispose();
                additionalQueuedHostedService.Dispose();
            }
        }

        AssertHandledBefore(activity, waitFinishedAt);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForRegisteredEventsToBeHandled()
    {
        var activity = new FakeApplicationActivityThatFiresAnEvent();

        var wasEventHandlerCalled = false;
        _engine.On<FakeApplicationEvent>(_ =>
        {
            wasEventHandlerCalled = true;
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.True(wasEventHandlerCalled);
        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task RegisteredEventsAreOnlyCalledOnce()
    {
        var activity = new FakeApplicationActivityThatFiresAnEvent();

        var eventHandlerCallCount = 0;
        _engine.On<FakeApplicationEvent>(_ =>
        {
            Interlocked.Increment(ref eventHandlerCallCount);
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.Equal(1, eventHandlerCallCount);
        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForUnhandledExceptionsFromActivities()
    {
        var activity = new FakeApplicationActivityThatThrows();

        var wasEventHandlerCalled = false;
        _engine.On<UnhandledExceptionEvent>(_ =>
        {
            wasEventHandlerCalled = true;
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.True(wasEventHandlerCalled);
        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task WaitForUnhandledExceptionsFromEvents()
    {
        var activity = new FakeApplicationActivityThatFiresAnEventThatThrows();

        var wasEventHandlerCalled = false;
        _engine.On<UnhandledExceptionEvent>(_ =>
        {
            wasEventHandlerCalled = true;
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity, CancellationToken.None);
        await _engine.Wait(activity, CancellationToken.None);

        Assert.True(wasEventHandlerCalled);
        Assert.True(activity.WasHandleCalled);
    }

    class FakeApplicationActivityThatResultsInSynchronizedActivities : IApplicationActivity
    {
        public FakeApplicationEventThatDispatchesSynchronizedActivities? ChildEvent { get; private set; }
        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            ChildEvent = new FakeApplicationEventThatDispatchesSynchronizedActivities();
            await Task.Delay(s_handleDelay, cancellationToken);

            await eventClient.Fire(ChildEvent, cancellationToken);
        }
    }

    class FakeApplicationEventThatDispatchesSynchronizedActivities : IApplicationEvent
    {
        public const int ActivityCount = 30;
        public List<FakeSynchronizedActivity> Activities { get; } = new();

        public async ValueTask Handle(IApplicationActivityEngine activityClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            for (var i = 0; i < ActivityCount; i++)
            {
                Activities.Add(new FakeSynchronizedActivity(200));
            }

            foreach (var activity in Activities)
            {
                await activityClient.Dispatch(activity, CancellationToken.None);
            }
        }
    }

    [Fact]
    public async Task WaitForSynchronizedTriggeringActivity()
    {
        var workers = new List<(QueuedHostedService, Task, CancellationTokenSource)>();

        for (var index = 0; index < 5; index++)
        {
            var additionalWorkerCancellation = new CancellationTokenSource();
            var additionalQueuedHostedService = ActivatorUtilities.CreateInstance<QueuedHostedService>(_host.Services);
            var additionalWorkerTask = additionalQueuedHostedService.StartAsync(_workerCancellation.Token);

            workers.Add((additionalQueuedHostedService, additionalWorkerTask, additionalWorkerCancellation));
        }

        var activity = new FakeApplicationActivityThatResultsInSynchronizedActivities();

        DateTimeOffset stoppedWaitingAt;
        try
        {
            await _engine.Dispatch(activity, CancellationToken.None);
            await _engine.Wait(activity, CancellationToken.None);
            stoppedWaitingAt = DateTimeOffset.Now;
        }
        finally
        {
            foreach (var (additionalQueuedHostedService, additionalWorkerTask, additionalWorkerCancellation) in workers)
            {
                additionalWorkerCancellation.Cancel();
                await additionalWorkerTask;

                additionalWorkerCancellation.Dispose();
                additionalWorkerTask.Dispose();
                additionalQueuedHostedService.Dispose();
            }
        }

        Assert.True(activity.ChildEvent!.Activities.All(entry => entry.HandleStoppedAt <= stoppedWaitingAt));
        AssertHandled(activity);
    }

    [Fact]
    public async Task WaitForSynchronizedActivities()
    {
        var secondWorkerCancellation = new CancellationTokenSource();
        var secondQueuedHostedService = ActivatorUtilities.CreateInstance<QueuedHostedService>(_host.Services);
        var secondWorkerTask = secondQueuedHostedService.StartAsync(_workerCancellation.Token);

        try
        {
            const int activityDelay = 200;

            const int activityCount = 5;
            var activities = new List<FakeSynchronizedActivity>();
            for (var i = 0; i < activityCount; i++)
            {
                activities.Add(new FakeSynchronizedActivity(activityDelay));
            }

            foreach (var activity in activities)
            {
                await _engine.Dispatch(activity, CancellationToken.None);
            }

            foreach (var activity in activities)
            {
                await _engine.Wait(activity, CancellationToken.None);
            }

            AssertHandled(activities, activityDelay);
        }
        finally
        {
            secondWorkerCancellation.Cancel();
            await secondWorkerTask;

            secondWorkerCancellation.Dispose();
            secondWorkerTask.Dispose();
        }
    }

    private static void AssertHandled(List<FakeSynchronizedActivity> activities, int activityDelay)
    {
        const int tolerance = 50;
        foreach (var activity in activities)
        {
            (activity.HandleStoppedAt - activity.HandleStartedAt).Should()
                .BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(activityDelay - tolerance));
        }

        var sortedActivities = activities.OrderBy(activity => activity.HandleStartedAt);

        var previousActivityStoppedAt = DateTimeOffset.MinValue;
        foreach (var activity in sortedActivities)
        {
            Assert.True(activity.HandleStartedAt >= previousActivityStoppedAt);
            previousActivityStoppedAt = activity.HandleStoppedAt;
        }
    }

    private static void AssertHandledBefore(FakeApplicationActivityTreeThatWaitsOnChildren fakeActivity, DateTimeOffset before)
    {
        Assert.True(fakeActivity.WasHandleCalled);
        Assert.True(fakeActivity.DidWaitThreadFinish);
        Assert.True(fakeActivity.WaitThreadFinishedAt <= before);
        Assert.True(fakeActivity.HandleFinishedAt <= before);
        foreach (var childEvent in fakeActivity.Children)
        {
            AssertHandledBefore(childEvent, fakeActivity.WaitThreadFinishedAt);
        }
    }

    private static void AssertHandledBefore(FakeApplicationEventTree fakeEvent, DateTimeOffset before)
    {
        Assert.True(fakeEvent.WasHandleCalled);
        Assert.True(fakeEvent.HandleFinishedAt <= before);
        foreach (var childActivity in fakeEvent.Children)
        {
            AssertHandledBefore(childActivity, before);
        }
    }

    private static void AssertHandledBefore(FakeApplicationActivityTree fakeActivity, DateTimeOffset before)
    {
        Assert.True(fakeActivity.WasHandleCalled);
        Assert.True(fakeActivity.HandleFinishedAt <= before);
        foreach (var childActivity in fakeActivity.Children)
        {
            AssertHandledBefore(childActivity, before);
        }
    }

    private static void AssertHandled(FakeApplicationActivityThatResultsInSynchronizedActivities fakeActivity)
    {
        Assert.NotNull(fakeActivity.ChildEvent);
        Assert.Equal(FakeApplicationEventThatDispatchesSynchronizedActivities.ActivityCount, fakeActivity.ChildEvent.Activities.Count);
        AssertHandled(fakeActivity.ChildEvent.Activities, 200);
    }

    private void AssertHandled(FakeApplicationActivityTree fakeActivity)
    {
        Assert.True(fakeActivity.WasHandleCalled);
        foreach (var childEvent in fakeActivity.Children)
        {
            AssertHandled(childEvent);
        }
    }

    private void AssertHandled(FakeApplicationEventTree fakeEvent)
    {
        Assert.True(fakeEvent.WasHandleCalled);
        foreach (var childActivity in fakeEvent.Children)
        {
            AssertHandled(childActivity);
        }
    }

    private class FakeApplicationActivityThatFiresAnEventThatCountsCalls : IApplicationActivity
    {
        public FakeApplicationEventThatCountsCalls ChildEvent { get; private set; } = null!;

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            ChildEvent = new FakeApplicationEventThatCountsCalls();
            await eventClient.Fire(ChildEvent, cancellationToken);
        }
    }

    private class FakeApplicationEventThatCountsCalls : IApplicationEvent
    {
        public int HandleCallCount;

        public async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            Interlocked.Increment(ref HandleCallCount);
        }
    }

    private class FakeApplicationActivityThatCountsCalls : IApplicationActivity
    {
        public int HandleCallCount;
        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            Interlocked.Increment(ref HandleCallCount);
        }
    }

    private class FakeApplicationActivityThatThrows : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            WasHandleCalled = true;

            throw new Exception();
        }
    }

    private class FakeApplicationActivityThatFiresAnEventThatThrows : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            WasHandleCalled = true;

            await eventClient.Fire(new FakeApplicationEventThatThrows(), cancellationToken);
        }
    }

    private class FakeApplicationEventThatThrows : IApplicationEvent
    {
        public async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            throw new Exception();
        }
    }

    private class FakeApplicationActivity : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }
        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            WasHandleCalled = true;
        }
    }

    class FakeApplicationActivityThatFiresAnEvent : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);

            WasHandleCalled = true;

            await eventClient.Fire(new FakeApplicationEvent(), cancellationToken);
        }
    }

    class FakeApplicationActivityTreeThatWaitsOnChildren : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }
        public DateTimeOffset HandleFinishedAt { get; private set; }
        public bool DidWaitThreadFinish { get; private set; }
        public DateTimeOffset WaitThreadFinishedAt { get; private set; }

        public ConcurrentBag<FakeApplicationEventTree> Children { get; } = new();

        private const int MaxFanOut = TreeFanOut;
        private static int s_count;

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            var childWaitingThread = new Thread(Start);
            childWaitingThread.Start();

            await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);

            if (s_count < MaxFanOut)
            {
                Children.Add(new FakeApplicationEventTree());
                Children.Add(new FakeApplicationEventTree());

                foreach (var applicationEvent in Children)
                {
                    await eventClient.Fire(applicationEvent, cancellationToken);
                }

                Interlocked.Increment(ref s_count);
            }

            WasHandleCalled = true;
            HandleFinishedAt = DateTimeOffset.Now;

            return;

            async void Start()
            {
                var waitToken = new ApplicationTaskWaitToken();
                try
                {
                    await eventClient.RegisterChildWaitToken(this, waitToken, cancellationToken);

                    await eventClient.Wait(this, cancellationToken, waitToken);
                }
                finally
                {
                    waitToken.Signal();
                }

                DidWaitThreadFinish = true;
                WaitThreadFinishedAt = DateTimeOffset.Now;
            }
        }
    }

    class FakeApplicationActivityTree : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }
        public DateTimeOffset HandleFinishedAt { get; private set; }
        public ConcurrentBag<FakeApplicationEventTree> Children { get; } = new();

        private const int MaxFanOut = TreeFanOut;
        private static int s_count;

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);

            if (s_count < MaxFanOut)
            {
                Children.Add(new FakeApplicationEventTree());
                Children.Add(new FakeApplicationEventTree());

                foreach (var applicationEvent in Children)
                {
                    await eventClient.Fire(applicationEvent, cancellationToken);
                }

                Interlocked.Increment(ref s_count);
            }

            WasHandleCalled = true;
            HandleFinishedAt = DateTimeOffset.Now;
        }
    }

    class FakeApplicationEventTree : IApplicationEvent
    {
        public bool WasHandleCalled { get; private set; }
        public DateTimeOffset HandleFinishedAt { get; private set; }
        public ConcurrentBag<FakeApplicationActivityTree> Children { get; } = new();

        private const int MaxFanOut = TreeFanOut;
        private static int s_count;
        public async ValueTask Handle(IApplicationActivityEngine activityClient, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);

            if (s_count < MaxFanOut)
            {
                Children.Add(new FakeApplicationActivityTree());
                Children.Add(new FakeApplicationActivityTree());

                foreach (var applicationActivity in Children)
                {
                    await activityClient.Dispatch(applicationActivity, cancellationToken);
                }

                Interlocked.Increment(ref s_count);
            }

            WasHandleCalled = true;
            HandleFinishedAt = DateTimeOffset.Now;
        }
    }

    private class FakeApplicationEvent : IApplicationEvent
    {
        public async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
        {
            await Task.Delay(s_handleDelay, cancellationToken);
        }
    }

    private class FakeSynchronizedActivity : IApplicationActivity, ISynchronized
    {
        private readonly int _delay;
        private static readonly SemaphoreSlim s_semaphore = new(1, 1);

        public FakeSynchronizedActivity(int delay)
        {
            _delay = delay;
        }

        public DateTimeOffset HandleStartedAt { get; private set; }
        public DateTimeOffset HandleStoppedAt { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
        {
            HandleStartedAt = DateTimeOffset.Now;
            await Task.Delay(_delay, cancellationToken);
            HandleStoppedAt = DateTimeOffset.Now;
        }

        public SemaphoreSlim GetSemaphore() => s_semaphore;
    }

    public void Dispose()
    {
        _workerCancellation.Cancel();
        _workerTask.Wait();

        _workerCancellation.Dispose();
        _workerTask.Dispose();
        _queuedHostedService.Dispose();
        _host.Dispose();

        GC.SuppressFinalize(this);
    }
}
