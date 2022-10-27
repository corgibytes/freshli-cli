using System;
using System.CommandLine;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AnalyzeRunner : CommandRunner<AnalyzeCommand, AnalyzeCommandOptions>
{
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly IConfiguration _configuration;
    private readonly IApplicationEventEngine _eventEngine;
    private readonly IResultsApi _resultsApi;

    public AnalyzeRunner(
        IServiceProvider serviceProvider, IRunner runner, IConfiguration configuration,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine,
        IResultsApi resultsApi
    ) : base(serviceProvider, runner)
    {
        _configuration = configuration;
        _activityEngine = activityEngine;
        _eventEngine = eventEngine;
        _resultsApi = resultsApi;
    }

    public override int Run(AnalyzeCommandOptions options, IConsole console)
    {
        _configuration.CacheDir = options.CacheDir;
        _configuration.GitPath = options.GitPath;

        _activityEngine.Dispatch(new StartAnalysisActivity
        {
            HistoryInterval = options.HistoryInterval,
            RepositoryBranch = options.Branch,
            RepositoryUrl = options.RepositoryLocation,
            UseCommitHistory = options.CommitHistory ? CommitHistory.Full : CommitHistory.AtInterval,
            RevisionHistoryMode =
                options.LatestOnly ? RevisionHistoryMode.OnlyLatestRevision : RevisionHistoryMode.AllRevisions
        });

        var exitStatus = 0;

        _eventEngine.On<AnalysisFailureLoggedEvent>(analysisFailure =>
        {
            console.Out.WriteLine("Analysis failed because: " + analysisFailure.ErrorEvent.ErrorMessage);
            exitStatus = 1;
        });

        Guid? apiAnalysisId = null;
        _eventEngine.On<AnalysisApiCreatedEvent>(createdEvent =>
        {
            apiAnalysisId = createdEvent.ApiAnalysisId;
            console.Out.WriteLine(
                "Results will be available at: " +
                _resultsApi.GetResultsUrl(createdEvent.ApiAnalysisId)
            );
        });

        _activityEngine.Wait();

        if (apiAnalysisId != null)
        {
            _activityEngine.Dispatch(new UpdateAnalysisStatusActivity(
                apiAnalysisId.Value,
                exitStatus == 0 ? "success" : "error"
            ));

            _activityEngine.Wait();
        }
        else
        {
            console.Out.WriteLine($"Unable to communicate with API. {nameof(apiAnalysisId)} is not set.");
            exitStatus = -1;
        }

        return exitStatus;
    }
}
