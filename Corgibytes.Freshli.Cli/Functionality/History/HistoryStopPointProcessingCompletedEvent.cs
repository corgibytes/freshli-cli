using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopPointProcessingCompletedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        var historyStopPoint = Parent?.HistoryStopPoint;
        _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

        var logger = eventClient.ServiceProvider
            .GetRequiredService<ILogger<HistoryStopPointProcessingCompletedEvent>>();
        logger.LogDebug("Completed processing history stop point {id}", historyStopPoint.Id);

        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Process);

        return ValueTask.CompletedTask;
    }
}
