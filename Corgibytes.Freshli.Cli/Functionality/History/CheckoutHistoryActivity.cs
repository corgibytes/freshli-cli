using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    public CheckoutHistoryActivity(IGitManager gitManager, string gitExecutablePath,
        string cacheDirectory, string repositoryId, string commitSha)
    {
        GitManager = gitManager;
        GitExecutablePath = gitExecutablePath;
        CacheDirectory = cacheDirectory;
        RepositoryId = repositoryId;
        CommitSha = commitSha;
    }

    public IGitManager GitManager { get; }
    public string GitExecutablePath { get; }
    public string CacheDirectory { get; }
    public string RepositoryId { get; }
    public string CommitSha { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var archiveLocation = GitManager.CreateArchive(
            RepositoryId,
            CacheDirectory,
            GitManager.ParseCommitSha(CommitSha),
            GitExecutablePath
        );

        eventClient.Fire(new HistoryStopCheckedOutEvent(new AnalysisLocation(archiveLocation)));
    }
}
