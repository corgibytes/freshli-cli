using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public IAnalysisLocation? AnalysisLocation { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        if (AnalysisLocation is { CommitId: { } })
        {
            eventClient.Dispatch(new CheckoutHistoryActivity(AnalysisId, AnalysisLocation));
        }
    }
}
