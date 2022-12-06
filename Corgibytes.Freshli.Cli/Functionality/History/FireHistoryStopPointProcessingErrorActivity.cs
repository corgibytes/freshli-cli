using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class FireHistoryStopPointProcessingErrorActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public FireHistoryStopPointProcessingErrorActivity(int historyStopPointId, Exception error)
    {
        HistoryStopPointId = historyStopPointId;
        Error = error;
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(HistoryStopPointId, Error));
    }

    public int HistoryStopPointId { get; }
    public Exception Error { get; }

    public int Priority
    {
        get
        {
            return 0;
        }
    }
}
