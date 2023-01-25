using System;
using System.Threading;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class DefaultCountdownEvent : ICountdownEvent
{
    private readonly object _syncLock = new();
    private readonly CountdownEvent _innerCountdownEvent;

    public event EventHandler<ICountdownEvent.CountChangedArgs>? CountChanged;

    public DefaultCountdownEvent(int initialCount)
    {
        _innerCountdownEvent = new CountdownEvent(initialCount);
    }

    public virtual void Dispose()
    {
        _innerCountdownEvent.Dispose();

        GC.SuppressFinalize(this);
    }

    public virtual int CurrentCount
    {
        get
        {
            lock (_syncLock)
            {
                return _innerCountdownEvent.CurrentCount;
            }
        }
    }

    public virtual int InitialCount
    {
        get
        {
            lock (_syncLock)
            {
                return _innerCountdownEvent.InitialCount;
            }
        }
    }

    public virtual bool IsSet
    {
        get
        {
            lock (_syncLock)
            {
                return _innerCountdownEvent.IsSet;
            }
        }
    }

    public virtual void AddCount()
    {
        lock (_syncLock)
        {
            _innerCountdownEvent.AddCount();
            NotifyAdd();
        }
    }

    public virtual void AddCount(int signalCount)
    {
        lock (_syncLock)
        {
            _innerCountdownEvent.AddCount(signalCount);
            NotifyAdd(signalCount);
        }
    }

    public virtual void Reset()
    {
        lock (_syncLock)
        {
            TrackAndNotifyChange(() => _innerCountdownEvent.Reset());
        }
    }

    public virtual void Reset(int count)
    {
        lock (_syncLock)
        {
            TrackAndNotifyChange(() => _innerCountdownEvent.Reset(count));
        }
    }

    public virtual bool Signal(int signalCount)
    {
        lock (_syncLock)
        {
            var result = _innerCountdownEvent.Signal(signalCount);
            NotifySignal(signalCount);
            return result;
        }
    }

    public virtual bool Signal()
    {
        lock (_syncLock)
        {
            var result = _innerCountdownEvent.Signal();
            NotifySignal();
            return result;
        }
    }

    public virtual bool TryAddCount()
    {
        lock (_syncLock)
        {
            var result = _innerCountdownEvent.TryAddCount();

            if (result)
            {
                NotifyAdd();
            }

            return result;
        }
    }

    public virtual bool TryAddCount(int signalCount)
    {
        lock (_syncLock)
        {
            var result = _innerCountdownEvent.TryAddCount(signalCount);

            if (result)
            {
                NotifyAdd(signalCount);
            }

            return result;
        }
    }

    public bool TrySignal()
    {
        lock (_syncLock)
        {
            try
            {
                _innerCountdownEvent.Signal();
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            NotifySignal();

            return true;
        }
    }

    public bool TrySignal(int signalCount)
    {
        lock (_syncLock)
        {
            try
            {
                _innerCountdownEvent.Signal(signalCount);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            NotifySignal(signalCount);

            return true;
        }
    }

    public virtual void Wait()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        _innerCountdownEvent.Wait();
    }

    public virtual void Wait(int millisecondsTimeout)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        _innerCountdownEvent.Wait(millisecondsTimeout);
    }

    public virtual void Wait(CancellationToken cancellationToken)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        _innerCountdownEvent.Wait(cancellationToken);
    }

    public virtual void Wait(TimeSpan timeout)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        _innerCountdownEvent.Wait(timeout);
    }

    public virtual void Wait(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        _innerCountdownEvent.Wait(millisecondsTimeout, cancellationToken);
    }

    public virtual void Wait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        _innerCountdownEvent.Wait(timeout, cancellationToken);
    }

    private void NotifyAdd(int value = 1)
    {
        NotifyChange(value);
    }

    private void NotifySignal(int value = 1)
    {
        NotifyChange(-value);
    }

    private void NotifyChange(int value)
    {
        CountChanged?.Invoke(this, new ICountdownEvent.CountChangedArgs(value));
    }

    private void TrackAndNotifyChange(Action action)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        var previousCount = _innerCountdownEvent.CurrentCount;
        action();
        // ReSharper disable once InconsistentlySynchronizedField
        var change = _innerCountdownEvent.CurrentCount - previousCount;
        if (change != 0)
        {
            NotifyChange(change);
        }
    }
}
