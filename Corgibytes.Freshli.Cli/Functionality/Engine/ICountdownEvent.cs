using System;
using System.Threading;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface ICountdownEvent : IDisposable
{
    public int CurrentCount { get; }
    public int InitialCount { get; }
    public bool IsSet { get; }
    public void AddCount();
    public void AddCount(int signalCount);
    public void Reset();
    public void Reset(int count);
    public bool Signal(int signalCount);
    public bool Signal();
    public bool TryAddCount();
    public bool TryAddCount(int signalCount);
    public bool TrySignal();
    public bool TrySignal(int signalCount);
    public void Wait();
    public void Wait(int millisecondsTimeout);
    public void Wait(CancellationToken cancellationToken);
    public void Wait(TimeSpan timeout);
    public void Wait(int millisecondsTimeout, CancellationToken cancellationToken);
    public void Wait(TimeSpan timeout, CancellationToken cancellationToken);

    public event EventHandler<CountChangedArgs>? CountChanged;

    public record struct CountChangedArgs(int Delta)
    {
        public int Delta { get; } = Delta;
    }
}

