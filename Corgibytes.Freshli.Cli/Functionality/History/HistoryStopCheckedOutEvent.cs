using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : IApplicationEvent
{
    public IAnalysisLocation AnalysisLocation { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new DetectAgentsForDetectManifestsActivity(AnalysisLocation));
    }
}
