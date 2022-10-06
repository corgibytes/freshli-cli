using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public HistoryIntervalStopFoundEvent(Guid analysisId, IHistoryStopData historyStopData)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid AnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public IHistoryStopData HistoryStopData { get; set; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new CreateApiHistoryStopActivity(AnalysisId, HistoryStopData));
}
