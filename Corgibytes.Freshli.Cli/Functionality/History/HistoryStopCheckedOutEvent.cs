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
    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask Parent { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<HistoryStopCheckedOutEvent>>();
        logger.LogDebug("Checked out history stop point {id}", Parent.HistoryStopPointId);

        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Archive);
        await eventClient.Dispatch(
            new DetectAgentsForDetectManifestsActivity(AnalysisId, Parent),
            cancellationToken);
    }
}
