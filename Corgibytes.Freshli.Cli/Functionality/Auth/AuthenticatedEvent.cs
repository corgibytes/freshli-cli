using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Auth;

public class AuthenticatedEvent : IApplicationEvent
{
    public required Guid AnalysisId { get; init; }
    public required string RepositoryUrl { get; init; }
    public required PersonEntity Person { get; init; }

    public async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        await eventClient.Dispatch(
            new DetermineProjectActivity
            {
                AnalysisId = AnalysisId,
                RepositoryUrl = RepositoryUrl,
                Person = Person
            },
            cancellationToken);
    }
}
