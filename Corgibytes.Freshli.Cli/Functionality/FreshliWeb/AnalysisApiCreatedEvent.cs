using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class AnalysisApiCreatedEvent : IApplicationEvent
{
    public Guid CachedAnalysisId { get; set; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new CloneGitRepositoryActivity(CachedAnalysisId));
    }
}
