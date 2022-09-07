using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly string _cacheDirectory;
    [JsonProperty] private readonly string _commitId;
    [JsonProperty] private readonly string _gitExecutablePath;
    [JsonProperty] private readonly IGitManager _gitManager;
    [JsonProperty] private readonly string _repositoryId;

    public CheckoutHistoryActivity(IGitManager gitManager, string gitExecutablePath,
        string cacheDirectory, string repositoryId, string commitSha)
    {
        _gitManager = gitManager;
        _gitExecutablePath = gitExecutablePath;
        _cacheDirectory = cacheDirectory;
        _repositoryId = repositoryId;
        _commitId = commitSha;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        _gitManager.CreateArchive(
            _repositoryId,
            _cacheDirectory,
            _gitManager.ParseCommitId(_commitId),
            _gitExecutablePath
        );

        eventClient.Fire(
            new HistoryStopCheckedOutEvent(new AnalysisLocation(_cacheDirectory, _repositoryId, _commitId)));
    }
}
