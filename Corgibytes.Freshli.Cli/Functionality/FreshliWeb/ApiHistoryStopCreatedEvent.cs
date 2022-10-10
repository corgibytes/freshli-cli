using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiHistoryStopCreatedEvent : IApplicationEvent
{
    public ApiHistoryStopCreatedEvent(Guid cachedAnalysisId, int historyStopPointId)
    {
        CachedAnalysisId = cachedAnalysisId;
        HistoryStopPointId = historyStopPointId;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid CachedAnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public int HistoryStopPointId { get; set; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new CheckoutHistoryActivity(CachedAnalysisId, HistoryStopPointId));
}
