using System;
using System.CommandLine;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Lib;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.CommandRunners;

public class AnalyzeRunnerTest
{
    private readonly Mock<IApplicationActivityEngine> _activityEngine;
    private readonly AnalyzeRunner _analyzeRunner;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IConsole> _console;
    private readonly Mock<IApplicationEventEngine> _eventEngine;
    private readonly AnalyzeCommandOptions _options;

    public AnalyzeRunnerTest()
    {
        _configuration = new Mock<IConfiguration>();
        _activityEngine = new Mock<IApplicationActivityEngine>();
        _eventEngine = new Mock<IApplicationEventEngine>();

        var serviceProvider = new Mock<IServiceProvider>();
        var runner = new Mock<IRunner>();
        var resultsApi = new Mock<IResultsApi>();

        _analyzeRunner = new AnalyzeRunner(serviceProvider.Object, runner.Object, _configuration.Object,
            _activityEngine.Object, _eventEngine.Object, resultsApi.Object);

        _options = new AnalyzeCommandOptions
        {
            Branch = "example-branch",
            CacheDir = "/path/to/cache/dir",
            CommitHistory = false,
            GitPath = "/path/to/git",
            HistoryInterval = "1m",
            LatestOnly = false,
            RepositoryLocation = "https://example.com/sample.git"
        };

        _console = new Mock<IConsole>();
        var outputWriter = new Mock<IStandardStreamWriter>();
        _console.Setup(mock => mock.Out).Returns(outputWriter.Object);
    }

    [Fact]
    public void RunIndicatesThatAnalysisIsComplete()
    {
        var apiAnalysisId = Guid.NewGuid();

        _eventEngine
            .Setup(mock => mock.On(It.IsAny<Action<AnalysisApiCreatedEvent>>()))
            .Callback<Action<AnalysisApiCreatedEvent>>(action => action(
                new AnalysisApiCreatedEvent { ApiAnalysisId = apiAnalysisId }
            ));

        var exitCode = _analyzeRunner.Run(_options, _console.Object);

        Assert.Equal(0, exitCode);

        _configuration.VerifySet(mock => mock.CacheDir = _options.CacheDir);
        _configuration.VerifySet(mock => mock.GitPath = _options.GitPath);

        _activityEngine.Verify(mock => mock.Dispatch(It.Is<StartAnalysisActivity>(value =>
            value.HistoryInterval == _options.HistoryInterval &&
            value.RepositoryBranch == _options.Branch &&
            value.RepositoryUrl == _options.RepositoryLocation &&
            value.RevisionHistoryMode == RevisionHistoryMode.AllRevisions &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));

        _activityEngine.Verify(mock => mock.Dispatch(It.Is<UpdateAnalysisStatusActivity>(value =>
            value.ApiAnalysisId == apiAnalysisId &&
            value.Status == "success"
        )));
    }

    [Fact]
    public void RunIndicatesThatAnalysisFailed()
    {
        var apiAnalysisId = Guid.NewGuid();

        _eventEngine
            .Setup(mock => mock.On(It.IsAny<Action<AnalysisFailureLoggedEvent>>()))
            .Callback<Action<AnalysisFailureLoggedEvent>>(action => action(
                new AnalysisFailureLoggedEvent(new UnhandledExceptionEvent(new Exception("example failure")))
            ));

        _eventEngine
            .Setup(mock => mock.On(It.IsAny<Action<AnalysisApiCreatedEvent>>()))
            .Callback<Action<AnalysisApiCreatedEvent>>(action => action(
                new AnalysisApiCreatedEvent { ApiAnalysisId = apiAnalysisId }
            ));

        var exitCode = _analyzeRunner.Run(_options, _console.Object);

        Assert.Equal(1, exitCode);

        _configuration.VerifySet(mock => mock.CacheDir = _options.CacheDir);
        _configuration.VerifySet(mock => mock.GitPath = _options.GitPath);

        _activityEngine.Verify(mock => mock.Dispatch(It.Is<StartAnalysisActivity>(value =>
            value.HistoryInterval == _options.HistoryInterval &&
            value.RepositoryBranch == _options.Branch &&
            value.RepositoryUrl == _options.RepositoryLocation &&
            value.RevisionHistoryMode == RevisionHistoryMode.AllRevisions &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));

        _activityEngine.Verify(mock => mock.Dispatch(It.Is<UpdateAnalysisStatusActivity>(value =>
            value.ApiAnalysisId == apiAnalysisId &&
            value.Status == "error"
        )));
    }

    [Fact]
    public void RunIndicatesThatCouldNotCallApi()
    {
        var exitCode = _analyzeRunner.Run(_options, _console.Object);

        Assert.Equal(-1, exitCode);

        _configuration.VerifySet(mock => mock.CacheDir = _options.CacheDir);
        _configuration.VerifySet(mock => mock.GitPath = _options.GitPath);

        _activityEngine.Verify(mock => mock.Dispatch(It.Is<StartAnalysisActivity>(value =>
            value.HistoryInterval == _options.HistoryInterval &&
            value.RepositoryBranch == _options.Branch &&
            value.RepositoryUrl == _options.RepositoryLocation &&
            value.RevisionHistoryMode == RevisionHistoryMode.AllRevisions &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));
    }
}
