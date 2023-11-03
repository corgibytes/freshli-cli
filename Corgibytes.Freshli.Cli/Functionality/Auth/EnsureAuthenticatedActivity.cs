using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Auth;

public class EnsureAuthenticatedActivity : IApplicationActivity
{
    public required Guid AnalysisId { get; init; }
    public required string RepositoryUrl { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

        var person = await resultsApi.GetPerson(cancellationToken);
        if (person != null)
        {
            await eventClient.Fire(
                new AuthenticatedEvent()
                {
                    AnalysisId = AnalysisId,
                    RepositoryUrl = RepositoryUrl,
                    Person = person
                },
                cancellationToken
            );
        }
        else
        {
            await eventClient.Fire(new NotAuthenticatedEvent(), cancellationToken);
        }
    }
}
