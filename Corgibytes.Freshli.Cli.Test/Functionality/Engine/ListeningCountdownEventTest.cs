using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Engine;

public abstract class ListeningCountdownEventTest
{
    public abstract class WithZeroInitialCount
    {
        public class WhenSourceHasZeroInitialCount : IDisposable
        {
            private readonly ICountdownEvent _sourceCountdownEvent = new DefaultCountdownEvent(0);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WhenSourceHasZeroInitialCount()
            {
                _countdownEvent = new ListeningCountdownEvent(_sourceCountdownEvent, 0);

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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(0, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.True(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCount()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.AddCount();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount(5);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCountWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.AddCount(5);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceReset()
            {
                _sourceCountdownEvent.Reset();
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(12, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(12, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceResetWithValue()
            {
                _sourceCountdownEvent.Reset(12);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Signal()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignal()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.Signal();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.False(_countdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCount()
            {
                Assert.False(_sourceCountdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.False(_countdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCountWithValue()
            {
                Assert.False(_sourceCountdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignal()
            {
                Assert.False(_countdownEvent.TrySignal());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignal()
            {
                Assert.False(_sourceCountdownEvent.TrySignal());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignalWithValue()
            {
                Assert.False(_countdownEvent.TrySignal(12));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignalWithValue()
            {
                Assert.False(_sourceCountdownEvent.TrySignal(12));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Wait()
            {
                _countdownEvent.Wait();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeout()
            {
                _countdownEvent.Wait(10);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithCancellationToken()
            {
                _countdownEvent.Wait(CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeout()
            {
                _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(10, CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(TimeSpan.FromMilliseconds(10), CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _countdownEvent.Dispose();
                _sourceCountdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        public class WhenSourceHasPositiveInitialCount : IDisposable
        {
            private readonly ICountdownEvent _sourceCountdownEvent = new DefaultCountdownEvent(1);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WhenSourceHasPositiveInitialCount()
            {
                _countdownEvent = new ListeningCountdownEvent(_sourceCountdownEvent, 0);

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
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(0, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.True(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCount()
            {
                _sourceCountdownEvent.AddCount();
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount(5);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCountWithValue()
            {
                _sourceCountdownEvent.AddCount(5);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceReset()
            {
                _sourceCountdownEvent.Reset();
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(13, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(13, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceResetWithValue()
            {
                _sourceCountdownEvent.Reset(12);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Signal()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignal()
            {
                _sourceCountdownEvent.Signal();
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalWithValueTooLarge()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalWithValueAfterAdd()
            {
                _sourceCountdownEvent.AddCount();
                _sourceCountdownEvent.Signal(2);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.False(_countdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCount()
            {
                Assert.True(_sourceCountdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.False(_countdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCountWithValue()
            {
                Assert.True(_sourceCountdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignal()
            {
                Assert.False(_countdownEvent.TrySignal());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignal()
            {
                Assert.True(_sourceCountdownEvent.TrySignal());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignalWithValue()
            {
                Assert.False(_countdownEvent.TrySignal(12));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignalWithValueTooLarge()
            {
                Assert.False(_sourceCountdownEvent.TrySignal(12));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignalWithValueAfterAdd()
            {
                _sourceCountdownEvent.AddCount();
                Assert.True(_sourceCountdownEvent.TrySignal(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Wait()
            {
                _countdownEvent.Wait();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeout()
            {
                _countdownEvent.Wait(10);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithCancellationToken()
            {
                _countdownEvent.Wait(CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeout()
            {
                _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(10, CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(TimeSpan.FromMilliseconds(10), CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _countdownEvent.Dispose();
                _sourceCountdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }
    }

    public abstract class WithPositiveInitialCount
    {
        public class WhenSourceHasZeroInitialCount : IDisposable
        {
            private readonly ICountdownEvent _sourceCountdownEvent = new DefaultCountdownEvent(0);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WhenSourceHasZeroInitialCount()
            {
                _countdownEvent = new ListeningCountdownEvent(_sourceCountdownEvent, 1);

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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(1, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.False(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                _countdownEvent.AddCount();
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCount()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.AddCount();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                _countdownEvent.AddCount(5);
                Assert.Equal(6, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(5, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCountWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.AddCount(5);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceReset()
            {
                _sourceCountdownEvent.Reset();
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(12, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(11, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceResetWithValue()
            {
                _sourceCountdownEvent.Reset(12);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(12, _lastChangeNotificationValue);
                Assert.Equal(13, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrue()
            {
                Assert.True(_countdownEvent.Signal());
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalse()
            {
                _countdownEvent.AddCount();
                Assert.False(_countdownEvent.Signal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignal()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.Signal();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValueTooLarge()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrueWithValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.True(_countdownEvent.Signal(2));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalseWithValueAfterAdd()
            {
                _countdownEvent.AddCount(2);
                Assert.False(_countdownEvent.Signal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.True(_countdownEvent.TryAddCount());
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCount()
            {
                Assert.False(_sourceCountdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.True(_countdownEvent.TryAddCount(2));
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(2, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCountWithValue()
            {
                Assert.False(_sourceCountdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignal()
            {
                Assert.True(_countdownEvent.TrySignal());
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignal()
            {
                Assert.False(_sourceCountdownEvent.TrySignal());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignalWithValueTooLarge()
            {
                Assert.False(_countdownEvent.TrySignal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignalWithValue()
            {
                Assert.False(_sourceCountdownEvent.TrySignal(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignalWithValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.True(_countdownEvent.TrySignal(2));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _countdownEvent.Dispose();
                _sourceCountdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        public class WhenSourceHasPositiveInitialCount : IDisposable
        {
            private readonly ICountdownEvent _sourceCountdownEvent = new DefaultCountdownEvent(1);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WhenSourceHasPositiveInitialCount()
            {
                _countdownEvent = new ListeningCountdownEvent(_sourceCountdownEvent, 1);

                _countdownEvent.CountChanged += (_, args) =>
                {
                    _wasEventHandlerCalled = true;
                    _lastChangeNotificationValue = args.Delta;
                };
            }

            [Fact]
            public void SignalAndThenSignalSource()
            {
                _countdownEvent.Signal();
                _sourceCountdownEvent.Signal();

                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalThenSignal()
            {
                _sourceCountdownEvent.Signal();
                _countdownEvent.Signal();

                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }


            [Fact]
            public void CurrentCount()
            {
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(1, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.False(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                _countdownEvent.AddCount();
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCount()
            {
                _sourceCountdownEvent.AddCount();
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(2, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                _countdownEvent.AddCount(5);
                Assert.Equal(7, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(5, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceAddCountWithValue()
            {
                _sourceCountdownEvent.AddCount(5);
                Assert.Equal(7, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(5, _lastChangeNotificationValue);
                Assert.Equal(6, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetAfterSourceSignal()
            {
                _sourceCountdownEvent.Signal();
                _wasEventHandlerCalled = false;
                _countdownEvent.Reset();
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceReset()
            {
                _sourceCountdownEvent.Reset();
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(2, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(13, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(11, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceResetWithValue()
            {
                _sourceCountdownEvent.Reset(12);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(11, _lastChangeNotificationValue);
                Assert.Equal(13, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrue()
            {
                _countdownEvent.Signal();
                Assert.True(_countdownEvent.Signal());
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalse()
            {
                Assert.False(_countdownEvent.Signal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalReturningTrue()
            {
                Assert.True(_sourceCountdownEvent.Signal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalReturningFalse()
            {
                _sourceCountdownEvent.AddCount();
                Assert.False(_sourceCountdownEvent.Signal());
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValueTooLarge()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalWithValueTooLarge()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _sourceCountdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(2, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrueWithValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.True(_countdownEvent.Signal(3));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-3, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalseWithValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.False(_countdownEvent.Signal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceSignalReturningTrueWithValueAfterAdd()
            {
                _sourceCountdownEvent.AddCount();
                Assert.True(_sourceCountdownEvent.Signal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
            }

            [Fact]
            public void SourceSignalReturningFalseWithValueAfterAdd()
            {
                _sourceCountdownEvent.AddCount(2);
                Assert.False(_sourceCountdownEvent.Signal(2));
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.True(_countdownEvent.TryAddCount());
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCount()
            {
                Assert.True(_sourceCountdownEvent.TryAddCount());
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(2, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.True(_countdownEvent.TryAddCount(2));
                Assert.Equal(4, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(2, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTryAddCountWithValue()
            {
                Assert.True(_sourceCountdownEvent.TryAddCount(2));
                Assert.Equal(4, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(2, _lastChangeNotificationValue);
            }

            [Fact]
            public void TrySignal()
            {
                Assert.True(_countdownEvent.TrySignal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignal()
            {
                Assert.True(_sourceCountdownEvent.TrySignal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
            }

            [Fact]
            public void TrySignalWithValueTooLarge()
            {
                Assert.False(_countdownEvent.TrySignal(12));
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignalWithValueTooLarge()
            {
                Assert.False(_sourceCountdownEvent.TrySignal(12));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(2, _countdownEvent.CurrentCount);
            }

            [Fact]
            public void TrySignalWithValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.True(_countdownEvent.TrySignal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SourceTrySignalWithValueAfterAdd()
            {
                _sourceCountdownEvent.AddCount();
                Assert.True(_sourceCountdownEvent.TrySignal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
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
                _sourceCountdownEvent.Signal();

                await task;
                Assert.True(task.IsCompletedSuccessfully);
                Assert.Equal(0, _sourceCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithMillisecondTimeout()
            {
                await Task.Run(() =>
                {
                    _countdownEvent.Wait(10);
                });

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
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

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithTimeSpanTimeout()
            {
                await Task.Run(() =>
                {
                    _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
                });

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
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

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
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

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
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

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
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

                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _sourceCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _countdownEvent.Dispose();
                _sourceCountdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }
    }
}
