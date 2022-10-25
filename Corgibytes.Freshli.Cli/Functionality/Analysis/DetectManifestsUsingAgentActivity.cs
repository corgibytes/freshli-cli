using System;
using System.Collections.Concurrent;
using System.Threading;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity, IMutexed
{
    private static readonly ConcurrentDictionary<string, Mutex> s_historyPointMutexes = new();

    public DetectManifestsUsingAgentActivity(Guid analysisId, int historyStopPointId,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);

        var manifestPaths = cacheDb.RetrieveCachedManifests(HistoryStopPointId, AgentExecutablePath);

        if (manifestPaths.Count == 0)
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath);
            manifestPaths = agentReader.DetectManifests(historyStopPoint?.LocalPath!);
            cacheDb.StoreCachedManifests(HistoryStopPointId, AgentExecutablePath, manifestPaths);
        }

        foreach (var manifestPath in manifestPaths)
        {
            eventClient.Fire(new ManifestDetectedEvent(AnalysisId, HistoryStopPointId, AgentExecutablePath,
                manifestPath));
        }
    }

    public Mutex GetMutex(IServiceProvider serviceProvider)
    {
        var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        // TODO create an exception class for this exception and write a test to cover it getting generated
        _ = historyStopPoint ?? throw new Exception($"Failed to retrieve history stop point {HistoryStopPointId}");

        var historyPointPath = historyStopPoint.LocalPath;
        EnsureHistoryPointMutexExists(historyPointPath);
        return s_historyPointMutexes[historyPointPath];
    }

    private static void EnsureHistoryPointMutexExists(string path)
    {
        if (!s_historyPointMutexes.ContainsKey(path))
        {
            s_historyPointMutexes[path] = new Mutex();
        }
    }
}
