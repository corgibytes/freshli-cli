using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface ISynchronized
{
    public ValueTask<SemaphoreSlim> GetSemaphore(IServiceProvider serviceProvider);
}
