using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class NoManifestsDetectedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }

    public NoManifestsDetectedEvent(Guid analysisId, IHistoryStopPointProcessingTask parent)
    {
        AnalysisId = analysisId;
        Parent = parent;
    }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
