using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopPointProcessingFailedEvent : UnhandledExceptionEvent, IHistoryStopPointProcessingTask
{
    public int HistoryStopPointId { get; }

    public HistoryStopPointProcessingFailedEvent(int historyStopPointId, Exception error) : base(error)
    {
        HistoryStopPointId = historyStopPointId;
    }
}
