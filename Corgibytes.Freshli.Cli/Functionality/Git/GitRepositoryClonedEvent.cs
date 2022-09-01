using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryClonedEvent : IApplicationEvent
{
    public IServiceProvider ServiceProvider = null!;

    public string GitRepositoryId { get; init; } = null!;

    public Guid AnalysisId { get; init; }

    public string GitPath { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        var cacheDb = ServiceProvider.GetRequiredService<ICacheDb>();
        var computeHistoryService = ServiceProvider.GetRequiredService<IComputeHistory>();
        var analysisLocation = ServiceProvider.GetRequiredService<IAnalysisLocation>();

        eventClient.Dispatch(new ComputeHistoryActivity(GitPath, cacheDb, computeHistoryService, AnalysisId, analysisLocation));
    }
}
