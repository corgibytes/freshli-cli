using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
<<<<<<< HEAD
    public string? GitCommitIdentifier { get; init; }
=======
    public Guid AnalysisId { get; init; }
>>>>>>> 17d03008d745c9ae27235640a7d65e25e5fdf50f
    public IAnalysisLocation? AnalysisLocation { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        if (AnalysisLocation is { CommitId: { } })
        {
            eventClient.Dispatch(new CheckoutHistoryActivity(AnalysisId, AnalysisLocation));
        }
    }
}
