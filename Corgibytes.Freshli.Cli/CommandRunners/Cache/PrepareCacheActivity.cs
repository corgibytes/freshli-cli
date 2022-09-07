using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class PrepareCacheActivity : IApplicationActivity
{
    public PrepareCacheActivity(string cacheDirectory, string repositoryUrl = "", string? repositoryBranch = null,
        string historyInterval = "", bool useCommitHistory = false, string gitPath = "")
    {
        CacheDirectory = cacheDirectory;
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;
        UseCommitHistory = useCommitHistory;
        GitPath = gitPath;
    }

    public string RepositoryUrl { get; init; }

    public string? RepositoryBranch { get; init; }

    public bool UseCommitHistory { get; init; }

    public string GitPath { get; init; }

    // TODO: Research how to use a value class here instead of a string
    public string HistoryInterval { get; init; }

    public string CacheDirectory { get; init; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = new CacheManager();
        Console.Out.WriteLine(CliOutput.CachePrepareCommandRunner_Run_Preparing_cache, CacheDirectory);
        try
        {
            cacheManager.Prepare(CacheDirectory).ToExitCode();
            var cacheDb = cacheManager.GetCacheDb(CacheDirectory);
            cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval,
                UseCommitHistory));
            eventClient.Fire(new CachePreparedEvent
            {
                GitPath = GitPath,
                CacheDirectory = CacheDirectory,
                RepositoryUrl = RepositoryUrl,
                RepositoryBranch = RepositoryBranch,
                HistoryInterval = HistoryInterval,
                UseCommitHistory = UseCommitHistory
            });
        }
        catch (CacheException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }
}
