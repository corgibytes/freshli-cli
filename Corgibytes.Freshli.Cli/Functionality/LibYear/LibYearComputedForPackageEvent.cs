using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class LibYearComputedForPackageEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public int PackageLibYearId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;

    public async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new CreateApiPackageLibYearActivity
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId,
            PackageLibYearId = PackageLibYearId,
            AgentExecutablePath = AgentExecutablePath
        });
}
