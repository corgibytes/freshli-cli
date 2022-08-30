using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public string? GitCommitIdentifier { get; init; }
    public IAnalysisLocation? AnalysisLocation { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
