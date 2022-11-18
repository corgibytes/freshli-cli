using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IMutexed
{
    public ValueTask<Mutex> GetMutex(IServiceProvider serviceProvider);
}
