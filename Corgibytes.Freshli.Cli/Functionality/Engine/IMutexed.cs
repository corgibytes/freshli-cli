using System;
using System.Threading;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IMutexed
{
    public Mutex GetMutex(IServiceProvider serviceProvider);
}
