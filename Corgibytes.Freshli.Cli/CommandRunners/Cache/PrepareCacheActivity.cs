using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class PrepareCacheActivity : IApplicationActivity
{
    public PrepareCacheActivity(string repositoryUrl = "", string? repositoryBranch = null,
        string historyInterval = "", CommitHistory useCommitHistory = CommitHistory.AtInterval)
    {
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;
        UseCommitHistory = useCommitHistory;
    }

    public string RepositoryUrl { get; init; }

    public string? RepositoryBranch { get; init; }

    public CommitHistory UseCommitHistory { get; init; }

    public string GitPath { get; init; }

    // TODO: Research how to use a value class here instead of a string
    public string HistoryInterval { get; init; }

    public string CacheDirectory { get; init; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        var cacheManager = new CacheManager(configuration);
        Console.Out.WriteLine(CliOutput.CachePrepareCommandRunner_Run_Preparing_cache, CacheDirectory);
        try
        {
            cacheManager.Prepare(CacheDirectory).ToExitCode();
            var cacheDb = cacheManager.GetCacheDb();
            cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval,
                UseCommitHistory));
            eventClient.Fire(new CachePreparedEvent
            {
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
