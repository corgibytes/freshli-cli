using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisStartedEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }

    public string CacheDir { get; init; } = null!;

    public string GitPath { get; init; } = null!;

    public string RepositoryUrl { get; init; } = null!;

    public string? Branch { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new CloneGitRepositoryActivity(RepositoryUrl, Branch, CacheDir, GitPath, AnalysisId));
}
