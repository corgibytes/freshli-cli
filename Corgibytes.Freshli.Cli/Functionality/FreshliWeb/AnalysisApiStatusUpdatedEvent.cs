using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class AnalysisApiStatusUpdatedEvent : ApplicationEventBase
{
    public AnalysisApiStatusUpdatedEvent(Guid apiAnalysisId, string status)
    {
        ApiAnalysisId = apiAnalysisId;
        Status = status;
    }

    public Guid ApiAnalysisId { get; }
    public string Status { get; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
