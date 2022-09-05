using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryClonedEvent : IApplicationEvent
{
    public string GitRepositoryId { get; init; } = null!;

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid AnalysisId { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        // TODO - Dispatch(ComputeHistoryActivity)
    }
}
