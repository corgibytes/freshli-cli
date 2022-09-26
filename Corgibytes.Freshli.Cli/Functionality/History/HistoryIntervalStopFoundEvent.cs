using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public IAnalysisLocation? AnalysisLocation { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        if (AnalysisLocation != null && AnalysisLocation.CommitId != null)
        {
            eventClient.Dispatch(new CheckoutHistoryActivity(AnalysisLocation));
        }
    }
}
