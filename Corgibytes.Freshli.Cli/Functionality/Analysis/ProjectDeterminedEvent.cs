using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ProjectDeterminedEvent : IApplicationEvent
{
    public required Guid AnalysisId { get; init; }
    public required string RepositoryUrl { get; init; }
    public required string ProjectSlug { get; init;}

    public async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        if (Directory.Exists(RepositoryUrl))
        {
            await eventClient.Dispatch(
                new VerifyGitRepositoryInLocalDirectoryActivity
                {
                    AnalysisId = AnalysisId,
                    ProjectSlug = ProjectSlug
                },
                cancellationToken
            );
        }
        else
        {
            await eventClient.Dispatch(
                new CloneGitRepositoryActivity
                {
                    AnalysisId = AnalysisId,
                    ProjectSlug = ProjectSlug
                },
                cancellationToken
            );
        }
    }
}
