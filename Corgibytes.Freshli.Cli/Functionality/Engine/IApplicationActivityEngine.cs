using System;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivityEngine
{
    public IServiceProvider ServiceProvider { get; }

    public ValueTask Dispatch(IApplicationActivity applicationActivity);
    public ValueTask Wait();
}
