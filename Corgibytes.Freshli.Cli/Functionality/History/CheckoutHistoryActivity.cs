using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly string _gitExecutablePath;
    [JsonProperty] private readonly IGitManager _gitManager;
    [JsonProperty] private readonly IAnalysisLocation _analysisLocation;

    public CheckoutHistoryActivity(IGitManager gitManager, string gitExecutablePath,
        IAnalysisLocation analysisLocation)
    {
        _gitManager = gitManager;
        _gitExecutablePath = gitExecutablePath;
        _analysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        if (_analysisLocation.CommitId != null)
        {
            _gitManager.CreateArchive(
                _analysisLocation.RepositoryId,
                _analysisLocation.CacheDirectory,
                _gitManager.ParseCommitId(_analysisLocation.CommitId),
                _gitExecutablePath
            );

            eventClient.Fire(new HistoryStopCheckedOutEvent{ AnalysisLocation = _analysisLocation });
        }
    }
}
