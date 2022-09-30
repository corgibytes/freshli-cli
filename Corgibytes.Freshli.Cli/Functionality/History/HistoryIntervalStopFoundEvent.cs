using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public IAnalysisLocation AnalysisLocation { get; set; }

    public HistoryIntervalStopFoundEvent(IAnalysisLocation analysisLocation) => AnalysisLocation = analysisLocation;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new CheckoutHistoryActivity(AnalysisLocation));
    }
}
