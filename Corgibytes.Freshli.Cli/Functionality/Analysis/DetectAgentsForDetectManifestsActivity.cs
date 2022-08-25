using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity
{
    [JsonProperty]
    private IAgentsDetector _agentsDetector;

    [JsonProperty]
    private string _repositoryId;

    [JsonProperty]
    private string _commitId;

    public DetectAgentsForDetectManifestsActivity(IAgentsDetector agentsDetector, string repositoryId, string commitId)
    {
        _agentsDetector = agentsDetector;
        _repositoryId = repositoryId;
        _commitId = commitId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        foreach (var agentPath in _agentsDetector.Detect())
        {
            eventClient.Fire(new AgentDetectedForDetectManifestEvent()
            {
                RepositoryId = _repositoryId,
                CommitId = _commitId,
                AgentPath = agentPath
            });
        }
    }
}
