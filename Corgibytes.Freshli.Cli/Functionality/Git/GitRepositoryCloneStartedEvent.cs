using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryCloneStartedEvent : IApplicationEvent
{
    public required Guid AnalysisId { get; init; }

    public async ValueTask Handle(IApplicationActivityEngine eventClient)
    {

    }
}
