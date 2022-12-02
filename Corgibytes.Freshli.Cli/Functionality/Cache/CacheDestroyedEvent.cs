using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Cache;

public class CacheDestroyedEvent : ApplicationEventBase
{
    public int ExitCode { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        return ValueTask.CompletedTask;
    }
}
