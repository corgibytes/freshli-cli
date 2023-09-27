using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class AnalysisApiCreatedEvent : ApplicationEventBase
{
    public Guid AnalysisId { get; init; }
    public Guid ApiAnalysisId { get; init; }
    public string RepositoryUrl { get; init; } = null!;

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        if (Directory.Exists(RepositoryUrl))
        {
            await eventClient.Dispatch(
                new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = AnalysisId },
                cancellationToken);
        }
        else
        {
            await eventClient.Dispatch(
                new CloneGitRepositoryActivity(AnalysisId),
                cancellationToken);
        }
    }
}
