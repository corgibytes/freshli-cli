using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class PackageFoundEvent : IApplicationEvent, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;
    public PackageURL Package { get; init; } = null!;

    public async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new ComputeLibYearForPackageActivity
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId,
            AgentExecutablePath = AgentExecutablePath,
            Package = Package
        });
}
