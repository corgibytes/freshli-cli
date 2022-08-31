using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    [JsonProperty] private IGitManager _gitManager;
    [JsonProperty] private string _gitExecutablePath;
    [JsonProperty] private string _cacheDirectory;
    [JsonProperty] private string _repositoryId;
    [JsonProperty] private string _commitSha;

    public CheckoutHistoryActivity(IGitManager gitManager, string gitExecutablePath,
        string cacheDirectory, string repositoryId, string commitSha)
    {
        _gitManager = gitManager;
        _gitExecutablePath = gitExecutablePath;
        _cacheDirectory = cacheDirectory;
        _repositoryId = repositoryId;
        _commitSha = commitSha;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        _gitManager.CreateArchive(
            _repositoryId,
            _cacheDirectory,
            _gitManager.ParseCommitSha(_commitSha),
            _gitExecutablePath
        );

        eventClient.Fire(new HistoryStopCheckedOutEvent(new AnalysisLocation(_repositoryId, _cacheDirectory)));
    }
}
