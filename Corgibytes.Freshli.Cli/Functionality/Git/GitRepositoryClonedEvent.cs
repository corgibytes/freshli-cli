using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryClonedEvent : IApplicationEvent
{
    public string GitRepositoryId { get; init; } = null!;

    public Guid AnalysisId { get; init; }

    public string GitPath { get; init; } = null!;

    public string CacheDir { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var computeHistoryService = eventClient.ServiceProvider.GetRequiredService<IComputeHistory>();
        var analysisLocation = new AnalysisLocation(CacheDir, GitRepositoryId);

        eventClient.Dispatch(new ComputeHistoryActivity(GitPath, cacheManager.GetCacheDb(CacheDir), computeHistoryService, AnalysisId, analysisLocation));
    }
}
