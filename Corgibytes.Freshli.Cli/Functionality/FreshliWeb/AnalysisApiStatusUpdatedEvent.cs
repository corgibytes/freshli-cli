using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class AnalysisApiStatusUpdatedEvent : IApplicationEvent
{
    public AnalysisApiStatusUpdatedEvent(Guid apiAnalysisId, string status)
    {
        ApiAnalysisId = apiAnalysisId;
        Status = status;
    }

    public Guid ApiAnalysisId { get; }
    public string Status { get; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
