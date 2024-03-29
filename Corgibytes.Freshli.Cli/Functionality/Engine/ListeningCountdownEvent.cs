﻿namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public sealed class ListeningCountdownEvent : DefaultCountdownEvent
{
    private readonly object _sourceCountdownSyncLock = new();
    private ICountdownEvent? _sourceCountdownEvent;

    public ListeningCountdownEvent(ICountdownEvent sourceCountdownEvent, int initialCount) : base(initialCount)
    {
        sourceCountdownEvent.Interlock(() =>
        {
            _sourceCountdownEvent = sourceCountdownEvent;
            _sourceCountdownEvent.CountChanged += OnSourceCountChanged;

            var sourceCount = _sourceCountdownEvent.CurrentCount;
            if (sourceCount > 0)
            {
                TryAddCount(sourceCount);
            }
        });
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
            base.Reset(count + _sourceCountdownEvent!.CurrentCount);
        }
    }

    public override void Dispose()
    {
        lock (_sourceCountdownSyncLock)
        {
            if (_sourceCountdownEvent != null)
            {
                _sourceCountdownEvent.CountChanged -= OnSourceCountChanged;
                _sourceCountdownEvent.Dispose();
            }
        }

        base.Dispose();
    }
}
