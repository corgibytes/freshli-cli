using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : IApplicationEvent
{
    public IAnalysisLocation AnalysisLocation { get; }

    public HistoryStopCheckedOutEvent(IAnalysisLocation analysisLocation) => AnalysisLocation = analysisLocation;

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
