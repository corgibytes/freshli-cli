using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly IGitManager _gitManager;
    [JsonProperty] private readonly IAnalysisLocation _analysisLocation;

    public CheckoutHistoryActivity(IGitManager gitManager, IAnalysisLocation analysisLocation)
    {
        _gitManager = gitManager;
        _analysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        if (_analysisLocation.CommitId != null)
        {
            _gitManager.CreateArchive(
                _analysisLocation.RepositoryId,
                _gitManager.ParseCommitId(_analysisLocation.CommitId)
            );

            eventClient.Fire(new HistoryStopCheckedOutEvent{ AnalysisLocation = _analysisLocation });
        }
    }
}
