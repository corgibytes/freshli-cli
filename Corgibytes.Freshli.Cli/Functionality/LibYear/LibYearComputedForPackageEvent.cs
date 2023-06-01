using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class LibYearComputedForPackageEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask Parent { get; init; }
    public required int PackageLibYearId { get; init; }
    public required string AgentExecutablePath { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new CreateApiPackageLibYearActivity
                {
                    AnalysisId = AnalysisId,
                    Parent = Parent,
                    PackageLibYearId = PackageLibYearId,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(Parent, error),
                cancellationToken);
        }
    }
}
