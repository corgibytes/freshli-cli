using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands.Analyze;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Lib;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Commands.Analyze;

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

        _analyzeRunner = new AnalyzeRunner(serviceProvider.Object, runner.Object, _configuration.Object,
            _activityEngine.Object, _eventEngine.Object);

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

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task RunIndicatesThatAnalysisIsComplete()
    {
        var exitCode = await _analyzeRunner.Run(_options, _console.Object, CancellationToken.None);

        Assert.Equal(0, exitCode);

        VerifyConfigurationValuesSetCorrectly();
        VerifyStartAnalysisActivityDispatched();
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task RunIndicatesThatAnalysisFailed()
    {
        SetupAnalysisFailureLoggedEvent();

        var exitCode = await _analyzeRunner.Run(_options, _console.Object, CancellationToken.None);

        Assert.Equal(1, exitCode);

        VerifyConfigurationValuesSetCorrectly();
        VerifyStartAnalysisActivityDispatched();
    }

    private void VerifyStartAnalysisActivityDispatched() =>
        _activityEngine.Verify(mock =>
            mock.Dispatch(
                It.Is<StartAnalysisActivity>(value =>
                    value.HistoryInterval == _options.HistoryInterval &&
                    value.RepositoryBranch == _options.Branch &&
                    value.RepositoryUrl == _options.RepositoryLocation &&
                    value.RevisionHistoryMode == RevisionHistoryMode.AllRevisions &&
                    value.UseCommitHistory == CommitHistory.AtInterval
                ),
                It.IsAny<CancellationToken>(),
                ApplicationTaskMode.Tracked
            )
        );

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
}
