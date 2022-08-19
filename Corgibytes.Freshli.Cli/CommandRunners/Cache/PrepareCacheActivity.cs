using System;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Resources;
using System.ComponentModel.DataAnnotations;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.CommandRunners.Cache;

public class PrepareCacheActivity : IApplicationActivityEngine, IApplicationActivity
{
    public PrepareCacheActivity()
    {
        CacheDirectory = "";
        RepositoryUrl = "";
        RepositoryBranch = "";
        HistoryInterval = "";


    }
    public PrepareCacheActivity(string cacheDirectory, string repositoryUrl, string? repositoryBranch, string historyInterval)
    {
        CacheDirectory = cacheDirectory;
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;

    }

    public PrepareCacheActivity(string cacheDirectory)
    {
        CacheDirectory = cacheDirectory;
        RepositoryUrl = "";
        HistoryInterval = "";
    }

    public string RepositoryUrl { get; set; }

    public string? RepositoryBranch { get; set; }

    // TODO: Research how to use a value class here instead of a string
    public string HistoryInterval { get; set; }

    public string CacheDirectory { get; set; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = new CacheManager();
        Console.Out.WriteLine(
            string.Format(CliOutput.CachePrepareCommandRunner_Run_Preparing_cache, CacheDirectory)
        );
        try
        {

            var result = cacheManager.Prepare(CacheDirectory).ToExitCode();
            var cacheDb = cacheManager.GetCacheDb(CacheDirectory);
            var id = cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval));
            eventClient.Fire(new CachePreparedEvent(id));


        }
        catch (CacheException e)
        {
            Console.Error.WriteLine(e.Message);

        }
    }

    public void Dispatch(IApplicationActivity applicationActivity)
    {
        var cacheManager = new CacheManager();
        Console.Out.WriteLine(
            string.Format(CliOutput.CachePrepareCommandRunner_Run_Preparing_cache, CacheDirectory)
        );
        try
        {
            var result = cacheManager.Prepare(CacheDirectory).ToExitCode();
            var cacheDb = cacheManager.GetCacheDb(CacheDirectory);
            var id = cacheDb.SaveAnalysis(new CachedAnalysis(RepositoryUrl, RepositoryBranch, HistoryInterval));

            var cachePreparedEvent = new CachePreparedEvent(id);




        }
        catch (CacheException e)
        {
            Console.Error.WriteLine(e.Message);

        }


    }

    public void Wait() => throw new NotImplementedException();
}
