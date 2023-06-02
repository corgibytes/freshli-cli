using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Engine;

[IntegrationTest]
public abstract class DefaultCountdownEventTest
{
    public class WithZeroInitialCount : IDisposable
    {
        private readonly ICountdownEvent _countdownEvent = new DefaultCountdownEvent(0);
        private bool _wasEventHandlerCalled;
        private int _lastChangeNotificationValue;

        public WithZeroInitialCount()
        {
            _countdownEvent.CountChanged += (_, args) =>
            {
                _wasEventHandlerCalled = true;
                _lastChangeNotificationValue = args.Delta;
            };
        }

        [Fact]
        public void CurrentCount()
        {
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void InitialCount()
        {
            Assert.Equal(0, _countdownEvent.InitialCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void IsSet()
        {
            Assert.True(_countdownEvent.IsSet);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void AddCount()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                _countdownEvent.AddCount();
            });
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void AddCountWithValue()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                _countdownEvent.AddCount(5);
            });
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void Reset()
        {
            _countdownEvent.Reset();
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void ResetWithValue()
        {
            _countdownEvent.Reset(12);
            Assert.Equal(12, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(12, _lastChangeNotificationValue);
        }

        [Fact]
        public void Signal()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                _countdownEvent.Signal();
            });
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void SignalWithValue()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                _countdownEvent.Signal(12);
            });
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void TryAddCount()
        {
            Assert.False(_countdownEvent.TryAddCount());
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void TryAddCountWithValue()
        {
            Assert.False(_countdownEvent.TryAddCount(2));
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void TrySignal()
        {
            Assert.False(_countdownEvent.TrySignal());
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void TrySignalWithValue()
        {
            Assert.False(_countdownEvent.TrySignal(2));
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void Wait()
        {
            _countdownEvent.Wait();
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void WaitWithMillisecondTimeout()
        {
            _countdownEvent.Wait(10);
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void WaitWithCancellationToken()
        {
            _countdownEvent.Wait(CancellationToken.None);
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void WaitWithTimeSpanTimeout()
        {
            _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void WaitWithMillisecondTimeoutAndCancellationToken()
        {
            _countdownEvent.Wait(10, CancellationToken.None);
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void WaitWithTimeSpanTimeoutAndCancellationToken()
        {
            _countdownEvent.Wait(TimeSpan.FromMilliseconds(10), CancellationToken.None);
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        public void Dispose()
        {
            _countdownEvent.Dispose();

            GC.SuppressFinalize(this);
        }
    }

    public class WithPositiveInitialCount : IDisposable
    {
        private readonly ICountdownEvent _countdownEvent = new DefaultCountdownEvent(1);
        private bool _wasEventHandlerCalled;
        private int _lastChangeNotificationValue;

        public WithPositiveInitialCount()
        {
            _countdownEvent.CountChanged += (_, args) =>
            {
                _wasEventHandlerCalled = true;
                _lastChangeNotificationValue = args.Delta;
            };
        }

        [Fact]
        public void CurrentCount()
        {
            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void InitialCount()
        {
            Assert.Equal(1, _countdownEvent.InitialCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void IsSet()
        {
            Assert.False(_countdownEvent.IsSet);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void AddCount()
        {
            _countdownEvent.AddCount();
            Assert.Equal(2, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(1, _lastChangeNotificationValue);
        }

        [Fact]
        public void AddCountWithValue()
        {
            _countdownEvent.AddCount(5);
            Assert.Equal(6, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(5, _lastChangeNotificationValue);
        }

        [Fact]
        public void Reset()
        {
            _countdownEvent.Reset();
            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void ResetAfterAdd()
        {
            _countdownEvent.AddCount();
            _countdownEvent.Reset();
            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-1, _lastChangeNotificationValue);
        }

        [Fact]
        public void ResetWithValue()
        {
            _countdownEvent.Reset(12);
            Assert.Equal(12, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(11, _lastChangeNotificationValue);
        }

        [Fact]
        public void ResetWithValueAfterAdd()
        {
            _countdownEvent.AddCount();
            _countdownEvent.Reset(12);
            Assert.Equal(12, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(10, _lastChangeNotificationValue);
        }

        [Fact]
        public void SignalReturningTrue()
        {
            Assert.True(_countdownEvent.Signal());
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-1, _lastChangeNotificationValue);
        }

        [Fact]
        public void SignalReturningFalse()
        {
            _countdownEvent.AddCount();
            Assert.False(_countdownEvent.Signal());
            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-1, _lastChangeNotificationValue);
        }

        [Fact]
        public void SignalWithValueTooLarge()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                _countdownEvent.Signal(12);
            });
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void SignalReturningTrueWithValidValueAfterAdd()
        {
            _countdownEvent.AddCount();
            Assert.True(_countdownEvent.Signal(2));
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-2, _lastChangeNotificationValue);
        }

        [Fact]
        public void SignalReturningFalseWithValidValueAfterAdd()
        {
            _countdownEvent.AddCount(2);
            Assert.False(_countdownEvent.Signal(2));
            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-2, _lastChangeNotificationValue);
        }

        [Fact]
        public void TryAddCount()
        {
            Assert.True(_countdownEvent.TryAddCount());
            Assert.Equal(2, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(1, _lastChangeNotificationValue);
        }

        [Fact]
        public void TryAddCountWithValue()
        {
            Assert.True(_countdownEvent.TryAddCount(2));
            Assert.Equal(3, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(2, _lastChangeNotificationValue);
        }

        [Fact]
        public void TrySignal()
        {
            Assert.True(_countdownEvent.TrySignal());
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-1, _lastChangeNotificationValue);
        }

        [Fact]
        public void TrySignalWithValueTooLarge()
        {
            Assert.False(_countdownEvent.TrySignal(2));
            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact]
        public void TrySignalWithValueAfterAdd()
        {
            _countdownEvent.AddCount();
            Assert.True(_countdownEvent.TrySignal(2));
            Assert.Equal(0, _countdownEvent.CurrentCount);
            Assert.True(_wasEventHandlerCalled);
            Assert.Equal(-2, _lastChangeNotificationValue);
        }

        [Fact(Timeout = 2000)]
        public async Task Wait()
        {
            var task = Task.Run(() =>
            {
                _countdownEvent.Wait();
            });

            await Task.Delay(TimeSpan.FromSeconds(1));
            _countdownEvent.Signal();

            await task;
            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithMillisecondTimeout()
        {
            await Task.Run(() =>
            {
                _countdownEvent.Wait(10);
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithCancellationToken()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10);

            // ReSharper disable once MethodSupportsCancellation
            await Task.Run(() =>
            {
                try
                {
                    _countdownEvent.Wait(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithTimeSpanTimeout()
        {
            await Task.Run(() =>
            {
                _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithMillisecondTimeoutAndCancellationTokenWhenTokenIsCancelled()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10);

            // ReSharper disable once MethodSupportsCancellation
            await Task.Run(() =>
            {
                try
                {
                    _countdownEvent.Wait(1000, cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithMillisecondTimeoutAndCancellationTokenWhenTimeoutExpires()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            // ReSharper disable once MethodSupportsCancellation
            await Task.Run(() =>
            {
                _countdownEvent.Wait(10, cancellationTokenSource.Token);
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithTimeSpanTimeoutAndCancellationTokenWhenTokenIsCancelled()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10);

            // ReSharper disable once MethodSupportsCancellation
            await Task.Run(() =>
            {
                try
                {
                    _countdownEvent.Wait(TimeSpan.FromMilliseconds(1000), cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        [Fact(Timeout = 500)]
        public async Task WaitWithTimeSpanTimeoutAndCancellationTokenWhenTimeoutExpires()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            // ReSharper disable once MethodSupportsCancellation
            await Task.Run(() =>
            {
                _countdownEvent.Wait(TimeSpan.FromMilliseconds(10), cancellationTokenSource.Token);
            });

            Assert.Equal(1, _countdownEvent.CurrentCount);
            Assert.False(_wasEventHandlerCalled);
        }

        public void Dispose()
        {
            _countdownEvent.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
