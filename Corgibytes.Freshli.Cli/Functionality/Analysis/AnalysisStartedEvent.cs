using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Auth;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisStartedEvent : ApplicationEventBase
{
    public Guid AnalysisId { get; init; }
    public required string RepositoryUrl { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        await eventClient.Dispatch(
            new EnsureAuthenticatedActivity
            {
                AnalysisId = AnalysisId,
                RepositoryUrl = RepositoryUrl
            },
            cancellationToken
        );
    }
}
