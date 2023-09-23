using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class NoPackagesFoundEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public IHistoryStopPointProcessingTask? Parent { get; }

    public NoPackagesFoundEvent(IHistoryStopPointProcessingTask? parent)
    {
        Parent = parent;
    }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
