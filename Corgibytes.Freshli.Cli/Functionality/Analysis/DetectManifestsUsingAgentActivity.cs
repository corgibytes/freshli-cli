using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public DetectManifestsUsingAgentActivity(Guid analysisId, int historyStopPointId,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        AgentExecutablePath = agentExecutablePath;
    }

    public int Priority
    {
        get { return 100; }
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string AgentExecutablePath { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        try
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
            await foreach (var manifestPath in agentReader.DetectManifests(historyStopPoint?.LocalPath!))
            {
                await eventClient.Fire(new ManifestDetectedEvent(AnalysisId, HistoryStopPointId, AgentExecutablePath,
                    manifestPath));
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(HistoryStopPointId, error));
        }
    }
}
