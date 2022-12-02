using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class LibYearComputedForPackageEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public int PackageLibYearId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;

    public override async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new CreateApiPackageLibYearActivity
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId,
            PackageLibYearId = PackageLibYearId,
            AgentExecutablePath = AgentExecutablePath
        });
}
