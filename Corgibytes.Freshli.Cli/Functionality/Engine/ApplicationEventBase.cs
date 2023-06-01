using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public abstract class ApplicationEventBase : IApplicationEvent
{
    public virtual int Priority
    {
        get { return 0; }
    }

    public abstract ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken);
}
