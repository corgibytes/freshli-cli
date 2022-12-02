using System;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEngine
{
    public IServiceProvider ServiceProvider { get; }

    public ValueTask<bool> AreOperationsPending<T>(Func<T, bool> query);
}
