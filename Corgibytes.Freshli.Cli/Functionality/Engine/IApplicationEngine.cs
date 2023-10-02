using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEngine
{
    public IServiceProvider ServiceProvider { get; }

    public ValueTask Wait(IApplicationTask task, CancellationToken cancellationToken, ApplicationTaskWaitToken? excluding = null);
    public ValueTask RegisterChildWaitToken(IApplicationTask task, ApplicationTaskWaitToken waitToken, CancellationToken cancellationToken);
}
