using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class CreateAnalysisApiActivityTest
{
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public CreateAnalysisApiActivityTest()
    {
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    [Fact]
    public void HandleSendsRequest()
    {
        var url = "anything";
        var branch = "anythingelse";
        var cacheDir = "/actually/a/cache/dir";
        var gitPath = "git";
        var api = new Mock<IResultsApi>();
        var apiAnalysisId = Guid.NewGuid();
        var cachedAnalysisId = Guid.NewGuid();
        api.Setup(mock => mock.CreateAnalysis(url)).Returns(apiAnalysisId);
        var activity = new CreateAnalysisApiActivity(cachedAnalysisId, url, branch, cacheDir, gitPath);

        _serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(api.Object);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<AnalysisApiCreatedEvent>(value =>
                value.CachedAnalysisId == cachedAnalysisId &&
                value.Url == url &&
                value.Branch == branch &&
                value.CacheDir == cacheDir &&
                value.GitPath == gitPath &&
                value.ApiAnalysisId == apiAnalysisId
                )));
    }
}
