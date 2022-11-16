using System;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity
{
    private readonly Guid _analysisId;
    private readonly int _historyStopPointId;

    public DetectAgentsForDetectManifestsActivity(Guid analysisId, int historyStopPointId)
    {
        _analysisId = analysisId;
        _historyStopPointId = historyStopPointId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentsDetector = eventClient.ServiceProvider.GetRequiredService<IAgentsDetector>();
        var agents = agentsDetector.Detect();

        if (agents.Count == 0)
        {
            eventClient.Fire(new NoAgentsDetectedFailureEvent { ErrorMessage = "Could not locate any agents" });
            return;
        }

        foreach (var agentPath in agentsDetector.Detect())
        {
            eventClient.Fire(new AgentDetectedForDetectManifestEvent(_analysisId, _historyStopPointId, agentPath));
        }
    }
}
