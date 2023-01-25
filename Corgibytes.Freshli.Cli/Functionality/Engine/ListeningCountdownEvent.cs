namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public sealed class ListeningCountdownEvent : DefaultCountdownEvent
{
    private readonly object _sourceCountdownSyncLock = new();
    private readonly ICountdownEvent _sourceCountdownEvent;

    public ListeningCountdownEvent(ICountdownEvent sourceCountdownEvent, int initialCount) : base(initialCount)
    {
        _sourceCountdownEvent = sourceCountdownEvent;
        _sourceCountdownEvent.CountChanged += OnSourceCountChanged;
        if (_sourceCountdownEvent.CurrentCount > 0)
        {
            TryAddCount(_sourceCountdownEvent.CurrentCount);
        }
    }

    private void OnSourceCountChanged(object? _, ICountdownEvent.CountChangedArgs args)
    {
        lock (_sourceCountdownSyncLock)
        {
            switch (args.Delta)
            {
                case < 0:
                    TrySignal(-args.Delta);
                    break;
                case > 0:
                    TryAddCount(args.Delta);
                    break;
            }
        }
    }

    public override void Reset()
    {
        Reset(InitialCount);
    }

    public override void Reset(int count)
    {
        lock (_sourceCountdownSyncLock)
        {
            base.Reset(count + _sourceCountdownEvent.CurrentCount);
        }
    }

    public override void Dispose()
    {
        _sourceCountdownEvent.CountChanged -= OnSourceCountChanged;
        _sourceCountdownEvent.Dispose();

        base.Dispose();
    }
}
