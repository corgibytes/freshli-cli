using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class PackageFoundEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string AgentExecutablePath { get; init; }
    public required PackageURL Package { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken) =>
        await eventClient.Dispatch(
            new ComputeLibYearForPackageActivity
            {
                AnalysisId = AnalysisId,
                Parent = Parent,
                AgentExecutablePath = AgentExecutablePath,
                Package = Package
            },
            cancellationToken
        );
}
