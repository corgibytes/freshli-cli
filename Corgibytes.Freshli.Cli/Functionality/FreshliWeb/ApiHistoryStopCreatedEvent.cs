using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiHistoryStopCreatedEvent : ApplicationEventBase
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

    public override async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new CheckoutHistoryActivity(CachedAnalysisId, HistoryStopPointId));
}
