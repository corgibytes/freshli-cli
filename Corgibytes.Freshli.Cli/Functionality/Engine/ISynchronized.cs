using System.Threading;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface ISynchronized
{
    public SemaphoreSlim GetSemaphore();
}
