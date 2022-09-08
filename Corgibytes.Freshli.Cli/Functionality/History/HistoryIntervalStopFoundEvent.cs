using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    private readonly IAgentManager _agentManager;
    private readonly string _agentPath;
    private readonly IAnalysisLocation _analysisLocation;
    private readonly string _manifestPath;

    public HistoryIntervalStopFoundEvent(
        IAgentManager agentManager, IAnalysisLocation analysisLocation, string manifestPath, string agentPath
    )
    {
        _agentManager = agentManager;
        _analysisLocation = analysisLocation;
        _manifestPath = manifestPath;
        _agentPath = agentPath;
    }

    public HistoryIntervalStopFoundEvent()
    {

    }
    public string? GitCommitIdentifier { get; init; }
    public IAnalysisLocation? AnalysisLocation { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new GenerateBillOfMaterialsActivity(_agentManager, _analysisLocation, _manifestPath, _agentPath));
    }
}
