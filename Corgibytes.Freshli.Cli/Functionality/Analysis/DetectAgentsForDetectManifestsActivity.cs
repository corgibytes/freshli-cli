using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectAgentsForDetectManifestsActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }

    public int Priority
    {
        get { return 100; }
    }

    public DetectAgentsForDetectManifestsActivity(Guid analysisId, int historyStopPointId)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        try
        {
            var agentsDetector = eventClient.ServiceProvider.GetRequiredService<IAgentsDetector>();
            var agents = agentsDetector.Detect();

            if (agents.Count == 0)
            {
                await eventClient.Fire(new NoAgentsDetectedFailureEvent {ErrorMessage = "Could not locate any agents"});
                return;
            }

            foreach (var agentPath in agents)
            {
                await eventClient.Fire(
                    new AgentDetectedForDetectManifestEvent(AnalysisId, HistoryStopPointId, agentPath));
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(HistoryStopPointId, error));
        }
    }
}
