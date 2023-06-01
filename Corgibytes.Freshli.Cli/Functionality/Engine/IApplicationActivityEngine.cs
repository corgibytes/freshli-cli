using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivityEngine : IApplicationEngine
{
    public ValueTask Dispatch(IApplicationActivity applicationActivity, CancellationToken cancellationToken, ApplicationTaskMode mode = ApplicationTaskMode.Tracked);
}
