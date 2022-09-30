using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public IHistoryStopData HistoryStopData { get; set; }

    public HistoryIntervalStopFoundEvent(IHistoryStopData historyStopData) => HistoryStopData = historyStopData;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new CheckoutHistoryActivity(HistoryStopData));
    }
}
