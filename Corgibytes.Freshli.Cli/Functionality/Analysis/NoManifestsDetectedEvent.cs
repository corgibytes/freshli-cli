using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class NoManifestsDetectedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public override string ToString()
    {
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;

        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name}";
    }
}
