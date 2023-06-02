using System;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public class ChildCountdownEvent : DefaultCountdownEvent
{
    private readonly object _parentCountdownSyncLock = new();
    private readonly ICountdownEvent _parentCountdownEvent;

    public ChildCountdownEvent(ICountdownEvent parentCountdownEvent, int initialCount) : base(initialCount)
    {
        _parentCountdownEvent = parentCountdownEvent;
        CountChanged += OnChildCountChanged;
    }

    private void OnChildCountChanged(object? _, ICountdownEvent.CountChangedArgs args)
    {
        lock (_parentCountdownSyncLock)
        {
            switch (args.Delta)
            {
                case < 0:
                    _parentCountdownEvent.TrySignal(-args.Delta);
                    break;
                case > 0:
                    _parentCountdownEvent.TryAddCount(args.Delta);
                    break;
            }
        }
    }

    public override void Dispose()
    {
        CountChanged -= OnChildCountChanged;
        base.Dispose();

        GC.SuppressFinalize(this);
    }
}
