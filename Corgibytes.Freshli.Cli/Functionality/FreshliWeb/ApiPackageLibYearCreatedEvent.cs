using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiPackageLibYearCreatedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public int PackageLibYearId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;

    public override async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        try
        {
            await eventClient.Dispatch(
                new ReportHistoryStopPointProgressActivity {HistoryStopPointId = HistoryStopPointId});
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(new FireHistoryStopPointProcessingErrorActivity(HistoryStopPointId, error));
        }
    }
}
