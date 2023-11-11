using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Auth;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Auth;

[UnitTest]
public class EnsureAuthenticatedActivityTest
{
    private readonly Guid _analysisId = Guid.NewGuid();
    private readonly string _repositoryUrl = "repo-url";
    private readonly EnsureAuthenticatedActivity _activity;
    private readonly CancellationToken _cancellationToken = new();
    private readonly Mock<IApplicationEventEngine> _engine = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly Mock<IResultsApi> _resultsApi = new();
    private readonly Mock<ICacheManager> _cacheManager = new();

    public EnsureAuthenticatedActivityTest()
    {
        _activity = new EnsureAuthenticatedActivity()
        {
            AnalysisId = _analysisId,
            RepositoryUrl = _repositoryUrl
        };

        _engine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(_resultsApi.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
    }

    [Fact]
    public async Task HandleWhenAuthenticated()
    {
        var person = new PersonEntity();

        _cacheManager.Setup(mock => mock.AreApiCredentialsPresent()).ReturnsAsync(true);
        _resultsApi.Setup(mock => mock.GetPerson(_cancellationToken)).ReturnsAsync(person);

        await _activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<AuthenticatedEvent>(value =>
                value.AnalysisId == _analysisId &&
                value.RepositoryUrl == _repositoryUrl &&
                value.Person == person
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task HandleWhenNotAuthenticated()
    {
        _cacheManager.Setup(mock => mock.AreApiCredentialsPresent()).ReturnsAsync(true);
        _resultsApi.Setup(mock => mock.GetPerson(_cancellationToken)).ReturnsAsync(null as PersonEntity);

        await _activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<NotAuthenticatedEvent>(value => value.ErrorMessage == "Failed to verify authentication credentials. Please try logging in using the `auth` command."),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact]
    public async Task  HandleWhenAuthCredentialsAreNotPresent()
    {
        _cacheManager.Setup(mock => mock.AreApiCredentialsPresent()).ReturnsAsync(false);
        _resultsApi.Setup(mock => mock.GetPerson(_cancellationToken)).ReturnsAsync(new PersonEntity());

        await _activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock => mock.Fire(
            It.Is<NotAuthenticatedEvent>(value => value.ErrorMessage == "Failed to verify authentication credentials. Please try logging in using the `auth` command."),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }
}
