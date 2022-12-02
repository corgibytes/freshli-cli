using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : ApplicationEventBase
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Archive);
        await eventClient.Dispatch(new DetectAgentsForDetectManifestsActivity(AnalysisId, HistoryStopPointId));
    }
}
