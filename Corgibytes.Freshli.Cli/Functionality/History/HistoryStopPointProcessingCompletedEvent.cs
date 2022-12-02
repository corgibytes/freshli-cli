using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopPointProcessingCompletedEvent : ApplicationEventBase
{
    public required int HistoryStopPointId { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Process);

        return ValueTask.CompletedTask;
    }
}
