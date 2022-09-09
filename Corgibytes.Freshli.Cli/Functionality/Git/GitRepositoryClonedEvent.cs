using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryClonedEvent : IApplicationEvent
{
    public string GitRepositoryId { get; init; } = null!;

    public Guid AnalysisId { get; init; }

    public string GitPath { get; init; } = null!;

    public string CacheDir { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
        var analysisLocation = new AnalysisLocation(CacheDir, GitRepositoryId);
        eventClient.Dispatch(new ComputeHistoryActivity(GitPath, CacheDir, AnalysisId, analysisLocation));
    }
}
