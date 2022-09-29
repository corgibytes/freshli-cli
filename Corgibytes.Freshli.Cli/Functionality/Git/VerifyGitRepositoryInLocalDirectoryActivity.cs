using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class VerifyGitRepositoryInLocalDirectoryActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }

    public void Handle(IApplicationEventEngine eventClient) => throw new System.NotImplementedException();
}

