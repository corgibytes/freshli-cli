using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class NoPackagesFoundEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }

    public NoPackagesFoundEvent(Guid analysisId, IHistoryStopPointProcessingTask parent)
    {
        AnalysisId = analysisId;
        Parent = parent;
    }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
