using System;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivityEngine
{
    public IServiceProvider ServiceProvider { get; }

    public void Dispatch(IApplicationActivity applicationActivity);
    public void Wait();
}
