using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiHistoryStopCreatedEvent : IApplicationEvent
{
    public ApiHistoryStopCreatedEvent(Guid cachedAnalysisId, IHistoryStopData historyStopData)
    {
        CachedAnalysisId = cachedAnalysisId;
        HistoryStopData = historyStopData;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid CachedAnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public IHistoryStopData HistoryStopData { get; set; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new CheckoutHistoryActivity(CachedAnalysisId, HistoryStopData));
}
