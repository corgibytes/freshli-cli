using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ReportHistoryStopPointProgressActivity: IApplicationActivity
{
    public required int HistoryStopPointId { get; init; }

    public int Priority
    {
        get { return 10; }
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        if (!await eventClient.AreOperationsPending<IHistoryStopPointProcessingTask>(item =>
                item.HistoryStopPointId == HistoryStopPointId))
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingCompletedEvent {HistoryStopPointId = HistoryStopPointId});
        }
    }
}
