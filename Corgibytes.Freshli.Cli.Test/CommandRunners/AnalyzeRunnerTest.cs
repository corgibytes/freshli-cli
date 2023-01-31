using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Threading.Tasks;
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
    public async Task RunIndicatesThatAnalysisIsComplete()
    {
        var apiAnalysisId = Guid.NewGuid();

        SetupAnalysisApiCreatedEvent(apiAnalysisId);

        var exitCode = await _analyzeRunner.Run(_options, _console.Object);

        Assert.Equal(0, exitCode);

        VerifyConfigurationValuesSetCorrectly();
        VerifyStartAnalysisActivityDispatched();
        VerifyApiStatusUpdated(apiAnalysisId, "success");
    }

    [Fact]
    public async Task RunIndicatesThatAnalysisFailed()
    {
        var apiAnalysisId = Guid.NewGuid();

        SetupAnalysisFailureLoggedEvent();
        SetupAnalysisApiCreatedEvent(apiAnalysisId);

        var exitCode = await _analyzeRunner.Run(_options, _console.Object);

        Assert.Equal(1, exitCode);

        VerifyConfigurationValuesSetCorrectly();
        VerifyStartAnalysisActivityDispatched();
        VerifyApiStatusUpdated(apiAnalysisId, "error");
    }

    [Fact]
    public async Task RunIndicatesThatCouldNotCallApi()
    {
        var exitCode = await _analyzeRunner.Run(_options, _console.Object);

        Assert.Equal(-1, exitCode);

        VerifyConfigurationValuesSetCorrectly();
        VerifyStartAnalysisActivityDispatched();
    }

    private void VerifyApiStatusUpdated(Guid apiAnalysisId, string status) =>
        _activityEngine.Verify(mock => mock.Dispatch(It.Is<UpdateAnalysisStatusActivity>(value =>
            value.ApiAnalysisId == apiAnalysisId &&
            value.Status == status
        )));

    private void VerifyStartAnalysisActivityDispatched() =>
        _activityEngine.Verify(mock => mock.Dispatch(It.Is<StartAnalysisActivity>(value =>
            value.HistoryInterval == _options.HistoryInterval &&
            value.RepositoryBranch == _options.Branch &&
            value.RepositoryUrl == _options.RepositoryLocation &&
            value.RevisionHistoryMode == RevisionHistoryMode.AllRevisions &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));

    private void VerifyConfigurationValuesSetCorrectly()
    {
        _configuration.VerifySet(mock => mock.CacheDir = _options.CacheDir);
        _configuration.VerifySet(mock => mock.GitPath = _options.GitPath);
    }

    private void SetupAnalysisFailureLoggedEvent() =>
        _eventEngine
            .Setup(mock => mock.On(It.IsAny<Func<AnalysisFailureLoggedEvent, ValueTask>>()))
            .Callback<Func<AnalysisFailureLoggedEvent, ValueTask>>(action => action(
                new AnalysisFailureLoggedEvent(new UnhandledExceptionEvent(new Exception("example failure")))
            ).AsTask().Wait());

    private void SetupAnalysisApiCreatedEvent(Guid apiAnalysisId) =>
        _eventEngine
            .Setup(mock => mock.On(It.IsAny<Func<AnalysisApiCreatedEvent, ValueTask>>()))
            .Callback<Func<AnalysisApiCreatedEvent, ValueTask>>(action => action(
                new AnalysisApiCreatedEvent { ApiAnalysisId = apiAnalysisId }
            ).AsTask().Wait());
}
