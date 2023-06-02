using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiPackageLibYearCreatedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask Parent { get; init; }
    public required int PackageLibYearId { get; init; }
    public required string AgentExecutablePath { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
