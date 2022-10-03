using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity
{
    public DetectManifestsUsingAgentActivity(Guid analysisId, IHistoryStopData historyStopData, string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopData HistoryStopData { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);
        foreach (var manifestPath in agentReader.DetectManifests(HistoryStopData.Path))
        {
            eventClient.Fire(new ManifestDetectedEvent(AnalysisId, HistoryStopData, AgentExecutablePath,
                manifestPath));
        }
    }
}
