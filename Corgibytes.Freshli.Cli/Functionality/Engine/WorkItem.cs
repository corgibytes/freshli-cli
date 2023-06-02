using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public readonly struct WorkItem
{
    public IApplicationTask ApplicationTask { get; }
    public Func<CancellationToken, ValueTask> Invoker { get; }

    public WorkItem(IApplicationTask applicationTask, Func<CancellationToken, ValueTask> invoker)
    {
        ApplicationTask = applicationTask ?? throw new ArgumentNullException(nameof(applicationTask));
        Invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
    }
}
