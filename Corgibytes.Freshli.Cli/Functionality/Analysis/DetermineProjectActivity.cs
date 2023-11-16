using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetermineProjectActivity : ApplicationActivityBase
{
    public required Guid AnalysisId { get; init; }
    public required string RepositoryUrl { get; init; }
    public required PersonEntity Person { get; init; }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        var configuredProjectSlug = configuration.ProjectSlug;

        if (Person.IsSetupComplete)
        {
            if (ShouldUseImplicitProjectValue(configuredProjectSlug))
            {
                var organization = Person.Organizations[0];
                var project = organization.Projects[0];
                var projectSlug = $"{organization.Nickname}/{project.Nickname}";

                configuration.ProjectSlug = projectSlug;

                await FireProjectDeterminedEvent(eventClient, projectSlug, cancellationToken);
            }
            else
            {
                if (IsProjectSlugProvided(configuredProjectSlug))
                {
                    if (DoesPersonHaveAccessToProject(configuredProjectSlug))
                    {
                        await FireProjectDeterminedEvent(eventClient, configuredProjectSlug, cancellationToken);
                    }
                    else
                    {
                        await FireProjectNotFoundEvent(eventClient, configuredProjectSlug, cancellationToken);
                    }
                }
                else
                {
                    await FireProjectNotSpecifiedEvent(eventClient, cancellationToken);
                }
            }
        }
        else
        {
            await eventClient.Fire(
                new AccountNotSetUpEvent(configuration.UiUrl),
                cancellationToken
            );
        }
    }

    private async Task FireProjectDeterminedEvent(IApplicationEventEngine eventClient, string projectSlug,
        CancellationToken cancellationToken) =>
        await eventClient.Fire(
            new ProjectDeterminedEvent
            {
                AnalysisId = AnalysisId,
                RepositoryUrl = RepositoryUrl,
                ProjectSlug = projectSlug
            },
            cancellationToken
        );

    private async Task FireProjectNotFoundEvent(IApplicationEventEngine eventClient, string projectSlug,
        CancellationToken cancellationToken) =>
        await eventClient.Fire(
            new ProjectNotFoundEvent(Person, projectSlug),
            cancellationToken
        );

    private async Task FireProjectNotSpecifiedEvent(IApplicationEventEngine eventClient,
        CancellationToken cancellationToken) =>
        await eventClient.Fire(new ProjectNotSpecifiedEvent(Person), cancellationToken);

    private bool ShouldUseImplicitProjectValue(string? configuredProjectSlug)
    {
        return !IsProjectSlugProvided(configuredProjectSlug) && DoesPersonHaveExactlyOneProject();
    }

    private bool IsProjectSlugProvided(string? configuredProjectSlug)
    {
        return configuredProjectSlug != null;
    }

    private bool DoesPersonHaveExactlyOneProject() => Person.Organizations is [{ Projects.Count: 1 }];

    private bool DoesPersonHaveAccessToProject([NotNullWhen(true)] string? configuredProjectSlug)
    {
        return Person.Organizations.Any(organization =>
            organization.Projects.Any(project =>
                $"{organization.Nickname}/{project.Nickname}" == configuredProjectSlug
            )
        );
    }
}
