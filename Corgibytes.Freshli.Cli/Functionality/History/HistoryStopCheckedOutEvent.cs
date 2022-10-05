using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : IApplicationEvent
{
    public IAnalysisLocation AnalysisLocation { get; init; } = null!;
    public Guid AnalysisId { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) =>
<<<<<<< HEAD
        eventClient.Dispatch(new DetectAgentsForDetectManifestsActivity(
            eventClient.ServiceProvider.GetRequiredService<IAgentsDetector>(),
            eventClient.ServiceProvider.GetRequiredService<IAgentManager>(),
            AnalysisLocation));
=======
        eventClient.Dispatch(new DetectAgentsForDetectManifestsActivity(AnalysisId, AnalysisLocation));
>>>>>>> 17d03008d745c9ae27235640a7d65e25e5fdf50f
}
