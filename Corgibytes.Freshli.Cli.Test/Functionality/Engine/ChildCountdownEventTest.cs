using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Engine;

[IntegrationTest]
public abstract class ChildCountdownEventTest
{
    public abstract class WithZeroInitialCount
    {
        public class WithParentAtCountZero : IDisposable
        {
            private readonly ICountdownEvent _parentCountdownEvent = new DefaultCountdownEvent(0);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WithParentAtCountZero()
            {
                _countdownEvent = new ChildCountdownEvent(_parentCountdownEvent, 0);

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
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(0, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.True(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount(5);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(12, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(12, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Signal()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.False(_countdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.False(_countdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Wait()
            {
                _countdownEvent.Wait();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeout()
            {
                _countdownEvent.Wait(10);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithCancellationToken()
            {
                _countdownEvent.Wait(CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeout()
            {
                _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(10, CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(TimeSpan.FromMilliseconds(10), CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _parentCountdownEvent.Dispose();
                _countdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        public class WithParentAtPositiveCount : IDisposable
        {
            private readonly ICountdownEvent _parentCountdownEvent = new DefaultCountdownEvent(1);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WithParentAtPositiveCount()
            {
                _countdownEvent = new ChildCountdownEvent(_parentCountdownEvent, 0);

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
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(0, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.True(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.AddCount(5);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(12, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(12, _lastChangeNotificationValue);
                Assert.Equal(13, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Signal()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal();
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValue()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    _countdownEvent.Signal(12);
                });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.False(_countdownEvent.TryAddCount());
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.False(_countdownEvent.TryAddCount(2));
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Wait()
            {
                _countdownEvent.Wait();
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeout()
            {
                _countdownEvent.Wait(10);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithCancellationToken()
            {
                _countdownEvent.Wait(CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeout()
            {
                _countdownEvent.Wait(TimeSpan.FromMicroseconds(10));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithMillisecondTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(10, CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void WaitWithTimeSpanTimeoutAndCancellationToken()
            {
                _countdownEvent.Wait(TimeSpan.FromMilliseconds(10), CancellationToken.None);
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _parentCountdownEvent.Dispose();
                _countdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }
    }

    public abstract class WithPositiveInitialCount
    {
        public class WithParentAtCountZero : IDisposable
        {
            private readonly ICountdownEvent _parentCountdownEvent = new DefaultCountdownEvent(0);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WithParentAtCountZero()
            {
                _countdownEvent = new ChildCountdownEvent(_parentCountdownEvent, 1);
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
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalse()
            {
                _countdownEvent.AddCount();
                Assert.False(_countdownEvent.Signal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValueTooLarge()
            {
                Assert.Throws<InvalidOperationException>(() => { _countdownEvent.Signal(12); });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrueWithValidValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.True(_countdownEvent.Signal(2));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalseWithValidValueAfterAdd()
            {
                _countdownEvent.AddCount(2);
                Assert.False(_countdownEvent.Signal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.True(_countdownEvent.TryAddCount());
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.True(_countdownEvent.TryAddCount(2));
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.Equal(2, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 2000)]
            public async Task Wait()
            {
                var task = Task.Run(() => { _countdownEvent.Wait(); });

                await Task.Delay(TimeSpan.FromSeconds(1));
                _countdownEvent.Signal();

                await task;
                Assert.True(task.IsCompletedSuccessfully);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithMillisecondTimeout()
            {
                await Task.Run(() => { _countdownEvent.Wait(10); });

                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithTimeSpanTimeout()
            {
                await Task.Run(() => { _countdownEvent.Wait(TimeSpan.FromMicroseconds(10)); });

                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithMillisecondTimeoutAndCancellationTokenWhenTimeoutExpires()
            {
                var cancellationTokenSource = new CancellationTokenSource();

                // ReSharper disable once MethodSupportsCancellation
                await Task.Run(() => { _countdownEvent.Wait(10, cancellationTokenSource.Token); });

                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _parentCountdownEvent.Dispose();
                _countdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        public class WithParentAtPositiveCount : IDisposable
        {
            private readonly ICountdownEvent _parentCountdownEvent = new DefaultCountdownEvent(1);
            private readonly ICountdownEvent _countdownEvent;
            private bool _wasEventHandlerCalled;
            private int _lastChangeNotificationValue;

            public WithParentAtPositiveCount()
            {
                _countdownEvent = new ChildCountdownEvent(_parentCountdownEvent, 1);
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
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void InitialCount()
            {
                Assert.Equal(1, _countdownEvent.InitialCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void IsSet()
            {
                Assert.False(_countdownEvent.IsSet);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCount()
            {
                _countdownEvent.AddCount();
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(2, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void AddCountWithValue()
            {
                _countdownEvent.AddCount(5);
                Assert.Equal(6, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(5, _lastChangeNotificationValue);
                Assert.Equal(6, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void Reset()
            {
                _countdownEvent.Reset();
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetAfterAdd()
            {
                _countdownEvent.AddCount();
                _countdownEvent.Reset();
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValue()
            {
                _countdownEvent.Reset(12);
                Assert.Equal(12, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(11, _lastChangeNotificationValue);
                Assert.Equal(12, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void ResetWithValueAfterAdd()
            {
                _countdownEvent.AddCount();
                _countdownEvent.Reset(12);
                Assert.Equal(12, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(10, _lastChangeNotificationValue);
                Assert.Equal(12, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrue()
            {
                Assert.True(_countdownEvent.Signal());
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalse()
            {
                _countdownEvent.AddCount();
                Assert.False(_countdownEvent.Signal());
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-1, _lastChangeNotificationValue);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalWithValueTooLarge()
            {
                Assert.Throws<InvalidOperationException>(() => { _countdownEvent.Signal(12); });
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningTrueWithValidValueAfterAdd()
            {
                _countdownEvent.AddCount();
                Assert.True(_countdownEvent.Signal(2));
                Assert.Equal(0, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void SignalReturningFalseWithValidValueAfterAdd()
            {
                _countdownEvent.AddCount(2);
                Assert.False(_countdownEvent.Signal(2));
                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(-2, _lastChangeNotificationValue);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCount()
            {
                Assert.True(_countdownEvent.TryAddCount());
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(2, _countdownEvent.CurrentCount);
                Assert.Equal(1, _lastChangeNotificationValue);
                Assert.Equal(2, _parentCountdownEvent.CurrentCount);
            }

            [Fact]
            public void TryAddCountWithValue()
            {
                Assert.True(_countdownEvent.TryAddCount(2));
                Assert.Equal(3, _countdownEvent.CurrentCount);
                Assert.True(_wasEventHandlerCalled);
                Assert.Equal(2, _lastChangeNotificationValue);
                Assert.Equal(3, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 2000)]
            public async Task Wait()
            {
                var task = Task.Run(() => { _countdownEvent.Wait(); });

                await Task.Delay(TimeSpan.FromSeconds(1));
                _countdownEvent.Signal();

                await task;
                Assert.True(task.IsCompletedSuccessfully);
                Assert.Equal(0, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithMillisecondTimeout()
            {
                await Task.Run(() => { _countdownEvent.Wait(10); });

                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithTimeSpanTimeout()
            {
                await Task.Run(() => { _countdownEvent.Wait(TimeSpan.FromMicroseconds(10)); });

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
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            [Fact(Timeout = 500)]
            public async Task WaitWithMillisecondTimeoutAndCancellationTokenWhenTimeoutExpires()
            {
                var cancellationTokenSource = new CancellationTokenSource();

                // ReSharper disable once MethodSupportsCancellation
                await Task.Run(() => { _countdownEvent.Wait(10, cancellationTokenSource.Token); });

                Assert.Equal(1, _countdownEvent.CurrentCount);
                Assert.False(_wasEventHandlerCalled);
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
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
                Assert.Equal(1, _parentCountdownEvent.CurrentCount);
            }

            public void Dispose()
            {
                _parentCountdownEvent.Dispose();
                _countdownEvent.Dispose();

                GC.SuppressFinalize(this);
            }
        }
    }
}
