using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        var historyStopPoint = Parent?.HistoryStopPoint;
        _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<HistoryStopCheckedOutEvent>>();
        logger.LogDebug("Checked out history stop point {id}", historyStopPoint.Id);

        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Archive);
        await eventClient.Dispatch(
            new DetectAgentsForDetectManifestsActivity { Parent = this },
            cancellationToken);
    }
}
