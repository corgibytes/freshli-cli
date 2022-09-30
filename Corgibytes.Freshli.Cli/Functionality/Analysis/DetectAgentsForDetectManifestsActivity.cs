using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity
{
    [JsonProperty] private readonly IHistoryStopData _historyStopData;

    public DetectAgentsForDetectManifestsActivity(IHistoryStopData historyStopData) =>
        _historyStopData = historyStopData;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentsDetector = eventClient.ServiceProvider.GetRequiredService<IAgentsDetector>();
        foreach (var agentPath in agentsDetector.Detect())
        {
            eventClient.Fire(new AgentDetectedForDetectManifestEvent(_historyStopData, agentPath));
        }
    }
}
