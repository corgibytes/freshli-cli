using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public abstract class ApplicationEventBase : ApplicationTaskBase, IApplicationEvent
{
    protected ApplicationEventBase() : base(0)
    {
    }

    protected ApplicationEventBase(int initialPriority) : base(initialPriority)
    {
    }

    public abstract ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken);
}
