using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity
{
    [JsonProperty] private IAgentsDetector _agentsDetector;
    [JsonProperty] private IAgentManager _agentManager;
    [JsonProperty] private IAnalysisLocation _analysisLocation;


    public DetectAgentsForDetectManifestsActivity(IAgentsDetector agentsDetector, IAgentManager agentManager, IAnalysisLocation analysisLocation)
    {
        _agentsDetector = agentsDetector;
        _agentManager = agentManager;
        _analysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        foreach (var agentPath in _agentsDetector.Detect())
        {
            eventClient.Fire(new AgentDetectedForDetectManifestEvent(
                _analysisLocation, _agentManager.GetReader(agentPath)));
        }
    }
}
