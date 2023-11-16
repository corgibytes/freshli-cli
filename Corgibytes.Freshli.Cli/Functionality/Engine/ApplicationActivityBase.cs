using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public abstract class ApplicationActivityBase : ApplicationTaskBase, IApplicationActivity
{
    protected ApplicationActivityBase() : base(1000)
    {
    }

    protected ApplicationActivityBase(int initialPriority) : base(initialPriority)
    {
    }

    public abstract ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken);
}
