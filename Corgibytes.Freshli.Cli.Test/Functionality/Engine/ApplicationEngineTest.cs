using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NLog.Extensions.Hosting;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Engine;

[IntegrationTest]
public class ApplicationEngineTest : IDisposable
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ApplicationEngine _engine;

    private readonly QueuedHostedService _queuedHostedService;
    private readonly Task _workerTask;
    private readonly CancellationTokenSource _workerCancellation;
    private readonly IConfiguration _configuration;
    private readonly IHost _host;

    public ApplicationEngineTest()
    {
        _configuration = new Configuration(new Environment());
        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(NullLoggerProvider.Instance);
            })
            .UseNLog()
            .ConfigureServices((_, services) =>
            {
                new FreshliServiceBuilder(services, _configuration).Register();
            })
            .Build();

        _workerCancellation = new CancellationTokenSource();
        _queuedHostedService = ActivatorUtilities.CreateInstance<QueuedHostedService>(_host.Services);
        _workerTask = _queuedHostedService.StartAsync(_workerCancellation.Token);

        _taskQueue = _host.Services.GetRequiredService<IBackgroundTaskQueue>();
        _engine = _host.Services.GetRequiredService<ApplicationEngine>();
    }

    [Fact(Timeout = 1000)]
    public async Task WaitForTaskCompletion()
    {
        var activity = new FakeApplicationActivity();
        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = 1000)]
    public async Task ActivityHandleMethodsAreOnlyCalledOnce()
    {
        var activity = new FakeApplicationActivityThatCountsCalls();
        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.Equal(1,activity.HandleCallCount);
    }

    [Fact(Timeout = 1000)]
    public async Task EventsAreOnlyFiredOnce()
    {
        var activity = new FakeApplicationActivityThatFiresAnEventThatCountsCalls();
        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.Equal(1,activity.ChildEvent.HandleCallCount);
    }

    [Fact(Timeout = 1000)]
    public async Task WaitForTaskCompletionWhenTaskHasChildren()
    {
        var activity = new FakeApplicationActivityThatFiresAnEvent();

        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = 1000)]
    public async Task WaitForTaskCompletionWithTreeOfActivitiesAndEvents()
    {
        var activity = new FakeApplicationActivityTree();
        await _engine.Dispatch(activity);

        await _engine.Wait(activity);

        AssertHandled(activity);
    }

    [Fact(Timeout = 1000)]
    public async Task WaitForRegisteredEventsToBeHandled()
    {
        var activity = new FakeApplicationActivityThatFiresAnEvent();

        var wasEventHandlerCalled = false;
        _engine.On<FakeApplicationEvent>((applicationEvent) =>
        {
            wasEventHandlerCalled = true;
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.True(wasEventHandlerCalled);
        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = 1000)]
    public async Task RegisteredEventsAreOnlyCalledOnce()
    {
        var activity = new FakeApplicationActivityThatFiresAnEvent();

        var eventHandlerCallCount = 0;
        _engine.On<FakeApplicationEvent>((applicationEvent) =>
        {
            Interlocked.Increment(ref eventHandlerCallCount);
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.Equal(1, eventHandlerCallCount);
        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = 1000)]
    public async Task WaitForUnhandledExceptionsFromActivities()
    {
        var activity = new FakeApplicationActivityThatThrows();

        var wasEventHandlerCalled = false;
        _engine.On<UnhandledExceptionEvent>((applicationEvent) =>
        {
            wasEventHandlerCalled = true;
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.True(wasEventHandlerCalled);
        Assert.True(activity.WasHandleCalled);
    }

    [Fact(Timeout = 1000)]
    public async Task WaitForUnhandledExceptionsFromEvents()
    {
        var activity = new FakeApplicationActivityThatFiresAnEventThatThrows();

        var wasEventHandlerCalled = false;
        _engine.On<UnhandledExceptionEvent>((applicationEvent) =>
        {
            wasEventHandlerCalled = true;
            return ValueTask.CompletedTask;
        });

        await _engine.Dispatch(activity);
        await _engine.Wait(activity);

        Assert.True(wasEventHandlerCalled);
        Assert.True(activity.WasHandleCalled);
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

    class FakeApplicationActivityThatFiresAnEventThatCountsCalls : IApplicationActivity
    {
        public FakeApplicationEventThatCountsCalls ChildEvent { get; private set; } = null!;

        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            ChildEvent = new FakeApplicationEventThatCountsCalls();
            await eventClient.Fire(ChildEvent);
        }
    }

    class FakeApplicationEventThatCountsCalls : IApplicationEvent
    {
        public int HandleCallCount;

        public async ValueTask Handle(IApplicationActivityEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Interlocked.Increment(ref HandleCallCount);
        }
    }

    class FakeApplicationActivityThatCountsCalls : IApplicationActivity
    {
        public int HandleCallCount = 0;
        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Interlocked.Increment(ref HandleCallCount);
        }
    }

    class FakeApplicationActivityThatThrows : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            WasHandleCalled = true;

            throw new Exception();
        }
    }

    class FakeApplicationActivityThatFiresAnEventThatThrows : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            WasHandleCalled = true;

            await eventClient.Fire(new FakeApplicationEventThatThrows());
        }
    }

    class FakeApplicationEventThatThrows : IApplicationEvent
    {
        public async ValueTask Handle(IApplicationActivityEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            throw new Exception();
        }
    }

    class FakeApplicationActivity : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }
        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            WasHandleCalled = true;
        }
    }

    class FakeApplicationActivityThatFiresAnEvent : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            WasHandleCalled = true;

            await eventClient.Fire(new FakeApplicationEvent());
        }
    }

    class FakeApplicationActivityTree : IApplicationActivity
    {
        public bool WasHandleCalled { get; private set; }
        public ConcurrentBag<FakeApplicationEventTree> Children { get; } = new();

        private const int MaxFanOut = 10;
        private static int s_count = 0;

        public async ValueTask Handle(IApplicationEventEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));

            if (s_count < MaxFanOut)
            {
                Children.Add(new FakeApplicationEventTree());
                Children.Add(new FakeApplicationEventTree());

                foreach (var applicationEvent in Children)
                {
                    await eventClient.Fire(applicationEvent);
                }

                Interlocked.Increment(ref s_count);
            }

            WasHandleCalled = true;
        }
    }

    class FakeApplicationEventTree : IApplicationEvent
    {
        public bool WasHandleCalled { get; private set; }
        public ConcurrentBag<FakeApplicationActivityTree> Children { get; } = new();

        private const int MaxFanOut = 10;
        private static int s_count = 0;
        public async ValueTask Handle(IApplicationActivityEngine activityClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));

            if (s_count < MaxFanOut)
            {
                Children.Add(new FakeApplicationActivityTree());
                Children.Add(new FakeApplicationActivityTree());

                foreach (var applicationActivity in Children)
                {
                    await activityClient.Dispatch(applicationActivity);
                }

                Interlocked.Increment(ref s_count);
            }

            WasHandleCalled = true;
        }
    }

    class FakeApplicationEvent : IApplicationEvent
    {
        public bool WasHandleCalled { get; private set; }

        public async ValueTask Handle(IApplicationActivityEngine eventClient)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            WasHandleCalled = true;
        }
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
