using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class GenerateBillOfMaterialsActivity : IApplicationActivity
{
    private readonly IAgentManager _agentManager;
    private readonly string _agentPath;
    private readonly IAnalysisLocation _analysisLocation;
    private readonly string _manifestPath;

    public GenerateBillOfMaterialsActivity(
        IAgentManager agentManager, IAnalysisLocation analysisLocation, string manifestPath, string agentPath
    )
    {
        _agentManager = agentManager;
        _analysisLocation = analysisLocation;
        _manifestPath = manifestPath;
        _agentPath = agentPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agent = _agentManager.GetReader(_agentPath);
        var asOfDate = DateTime.Now;
        var bla = agent.ProcessManifest(_manifestPath, asOfDate);

        eventClient.Fire(new BillOfMaterialsGeneratedEvent(_analysisLocation, bla));
    }
}
