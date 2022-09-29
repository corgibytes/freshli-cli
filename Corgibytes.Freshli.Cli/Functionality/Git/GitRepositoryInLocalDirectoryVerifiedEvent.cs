using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryInLocalDirectoryVerifiedEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public AnalysisLocation AnalysisLocation { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) => throw new System.NotImplementedException();
}
