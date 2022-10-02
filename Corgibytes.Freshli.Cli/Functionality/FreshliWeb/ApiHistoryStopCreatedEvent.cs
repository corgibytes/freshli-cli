using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiHistoryStopCreatedEvent : IApplicationEvent
{
    public Guid CachedAnalysisId { get; set; }
    public IHistoryStopData HistoryStopData { get; set; }

    public ApiHistoryStopCreatedEvent(Guid cachedAnalysisId, IHistoryStopData historyStopData)
    {
        CachedAnalysisId = CachedAnalysisId;
        HistoryStopData = historyStopData;
    }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new CheckoutHistoryActivity(HistoryStopData));
    }
}
