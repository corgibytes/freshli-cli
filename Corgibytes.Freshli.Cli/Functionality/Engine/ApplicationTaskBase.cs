using System.Threading;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public abstract class ApplicationTaskBase : IApplicationTask
{
    private int _priority = 0;
    public virtual int Priority => _priority;

    protected ApplicationTaskBase(int initialPriority)
    {
        _priority = initialPriority;
    }

    public void DecreasePriority() => Interlocked.Increment(ref _priority);
}
