using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class FireHistoryStopPointProcessingErrorActivity : ApplicationActivityBase, IHistoryStopPointProcessingTask
{
    public FireHistoryStopPointProcessingErrorActivity(IHistoryStopPointProcessingTask? parent, Exception error)
    {
        Parent = parent;
        Error = error;
    }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(this, Error), cancellationToken);
    }

    public IHistoryStopPointProcessingTask? Parent { get; }
    public Exception Error { get; }

    public int Priority
    {
        get
        {
            return 0;
        }
    }

    public override string ToString()
    {
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;

        var manifestId = Parent?.Manifest?.Id ?? 0;
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - Manifest = {manifestId}";
    }

}
