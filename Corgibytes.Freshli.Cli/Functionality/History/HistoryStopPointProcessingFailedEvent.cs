using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopPointProcessingFailedEvent : UnhandledExceptionEvent, IHistoryStopPointProcessingTask
{
    public int HistoryStopPointId { get; }

    public HistoryStopPointProcessingFailedEvent(int historyStopPointId, Exception error) : base(error)
    {
        HistoryStopPointId = historyStopPointId;
    }
}
