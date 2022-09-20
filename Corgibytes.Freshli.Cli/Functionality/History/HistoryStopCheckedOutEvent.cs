using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : IApplicationEvent
{
    public IAnalysisLocation AnalysisLocation { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new DetectAgentsForDetectManifestsActivity(
            eventClient.ServiceProvider.GetRequiredService<IAgentsDetector>(),  eventClient.ServiceProvider.GetRequiredService<IAgentManager>(),
            AnalysisLocation));
    }
}
