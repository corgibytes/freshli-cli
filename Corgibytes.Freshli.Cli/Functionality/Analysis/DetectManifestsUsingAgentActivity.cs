using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity
{
    public DetectManifestsUsingAgentActivity(IAnalysisLocation analysisLocation, string agentExecutablePath)
    {
        AnalysisLocation = analysisLocation;
        AgentExecutablePath = agentExecutablePath;
    }

    public IAnalysisLocation AnalysisLocation { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);
        foreach (var manifestPath in agentReader.DetectManifests(AnalysisLocation.Path))
        {
            eventClient.Fire(new ManifestDetectedEvent(AnalysisLocation, AgentExecutablePath, manifestPath));
        }
    }
}
