using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public HistoryIntervalStopFoundEvent(Guid analysisId, int historyStopPointId)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid AnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public int HistoryStopPointId { get; set; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new CreateApiHistoryStopActivity(AnalysisId, HistoryStopPointId));
}
