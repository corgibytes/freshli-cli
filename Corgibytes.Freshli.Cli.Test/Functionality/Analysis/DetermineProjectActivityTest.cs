using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class DetermineProjectActivityTest
{
    private readonly Guid _analysisId = Guid.NewGuid();
    private readonly string _repositoryUrl = "repo-url";
    private readonly Person _person = new();
    private readonly Mock<IApplicationEventEngine> _engine = new();
    private readonly CancellationToken _cancellationToken = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public DetermineProjectActivityTest()
    {
        _engine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _configuration.Setup(mock => mock.UiUrl).Returns("https://ui.freshli-staging.io");
    }

    [Fact]
    public async Task HandleWhenAccountIsNotSetup()
    {
        _person.IsSetupComplete = false;

        var activity = new DetermineProjectActivity
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl,
            Person = _person
        };

        await activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<AccountNotSetUpEvent>(value =>
                value.ErrorMessage == "Account is not setup. Please log-in to https://ui.freshli-staging.io and finish setting up your account."
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task HandleWhenAccountHasOnlyOneProjectAndProjectValueInConfigurationIsUndefined()
    {
        _person.IsSetupComplete = true;
        _person.Organizations = new List<Organization>
        {
            new()
            {
                Nickname = "test-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "test-project" }
                }
            }
        };

        var activity = new DetermineProjectActivity
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl,
            Person = _person
        };

        await activity.Handle(_engine.Object, _cancellationToken);

        _configuration.VerifySet(mock => mock.ProjectSlug = "test-org/test-project", Times.Once);

        _engine.Verify(mock => mock.Fire(
            It.Is<ProjectDeterminedEvent>(value =>
                value.AnalysisId == _analysisId &&
                value.RepositoryUrl == _repositoryUrl &&
                value.ProjectSlug == "test-org/test-project"
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task HandleWhenAccountHasOnlyOneProjectAndProjectValueInConfigurationDoesNotMatch()
    {
        _person.IsSetupComplete = true;
        _person.Organizations = new List<Organization>
        {
            new()
            {
                Nickname = "test-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "test-project" }
                }
            }
        };

        var activity = new DetermineProjectActivity
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl,
            Person = _person
        };

        _configuration.Setup(mock => mock.ProjectSlug).Returns("second-org/second-org-first-project");

        await activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<ProjectNotFoundEvent>(value =>
                value.ErrorMessage ==
                """
                Unable to find the project 'second-org/second-org-first-project'.
                Available options are:
                  * test-org/test-project
                """
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task HandleWhenProjectValueInConfigurationMatchesAvailableProject()
    {
        _person.IsSetupComplete = true;
        _person.Organizations = new List<Organization>
        {
            new()
            {
                Nickname = "first-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "first-org-first-project" },
                    new() { Nickname = "first-org-second-project" }
                }
            },
            new()
            {
                Nickname = "second-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "second-org-first-project" },
                    new() { Nickname = "second-org-second-project" }
                }
            }
        };

        _configuration.Setup(mock => mock.ProjectSlug).Returns("second-org/second-org-first-project");

        var activity = new DetermineProjectActivity
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl,
            Person = _person
        };

        await activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<ProjectDeterminedEvent>(value =>
                value.AnalysisId == _analysisId &&
                value.RepositoryUrl == _repositoryUrl &&
                value.ProjectSlug == "second-org/second-org-first-project"
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task HandleWhenProjectValueInConfigurationNotPresentInAvailableProject()
    {
        _person.IsSetupComplete = true;
        _person.Organizations = new List<Organization>
        {
            new()
            {
                Nickname = "first-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "first-org-first-project" },
                    new() { Nickname = "first-org-second-project" }
                }
            },
            new()
            {
                Nickname = "second-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "second-org-first-project" },
                    new() { Nickname = "second-org-second-project" }
                }
            }
        };

        _configuration.Setup(mock => mock.ProjectSlug).Returns("third-org/third-org-second-project");

        var activity = new DetermineProjectActivity
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl,
            Person = _person
        };

        await activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<ProjectNotFoundEvent>(value =>
                value.ErrorMessage ==
                    """
                    Unable to find the project 'third-org/third-org-second-project'.
                    Available options are:
                      * first-org/first-org-first-project
                      * first-org/first-org-second-project
                      * second-org/second-org-first-project
                      * second-org/second-org-second-project
                    """
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task HandleWhenProjectValueInConfigurationNotDefinedAndMultipleProjectsAvailable()
    {
        _person.IsSetupComplete = true;
        _person.Organizations = new List<Organization>
        {
            new()
            {
                Nickname = "first-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "first-org-first-project" },
                    new() { Nickname = "first-org-second-project" }
                }
            },
            new()
            {
                Nickname = "second-org",
                Projects = new List<Project>
                {
                    new() { Nickname = "second-org-first-project" },
                    new() { Nickname = "second-org-second-project" }
                }
            }
        };

        var activity = new DetermineProjectActivity
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl,
            Person = _person
        };

        await activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<ProjectNotSpecifiedEvent>(value =>
                value.ErrorMessage ==
                """
                The --project option is required when multiple projects are available.
                Please specify one of the following projects:
                  * first-org/first-org-first-project
                  * first-org/first-org-second-project
                  * second-org/second-org-first-project
                  * second-org/second-org-second-project
                """
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }
}
