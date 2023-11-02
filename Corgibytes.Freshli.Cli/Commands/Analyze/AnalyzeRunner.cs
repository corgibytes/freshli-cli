using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Analyze;

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

    public override async ValueTask<int> Run(AnalyzeCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        _configuration.CacheDir = options.CacheDir;
        _configuration.GitPath = options.GitPath;

        var startAnalysisActivity = new StartAnalysisActivity
        {
            HistoryInterval = options.HistoryInterval,
            RepositoryBranch = options.Branch,
            RepositoryUrl = options.RepositoryLocation,
            UseCommitHistory = options.CommitHistory ? CommitHistory.Full : CommitHistory.AtInterval,
            RevisionHistoryMode =
                options.LatestOnly ? RevisionHistoryMode.OnlyLatestRevision : RevisionHistoryMode.AllRevisions
        };

        var exitStatus = 0;

        _eventEngine.On<AnalysisFailureLoggedEvent>(analysisFailure =>
        {
            console.Out.WriteLine("Analysis failed because: " + analysisFailure.ErrorEvent.ErrorMessage);
            if (analysisFailure.ErrorEvent.Exception != null)
            {
                console.Out.WriteLine(analysisFailure.ErrorEvent.Exception.ToString());
            }

            exitStatus = 1;
            return ValueTask.CompletedTask;
        });

        Guid? apiAnalysisId = null;
        _eventEngine.On<AnalysisApiCreatedEvent>(createdEvent =>
        {
            apiAnalysisId = createdEvent.ApiAnalysisId;
            console.Out.WriteLine(
                "Results will be available at: " +
                _resultsApi.GetResultsUrl(createdEvent.ApiAnalysisId)
            );
            return ValueTask.CompletedTask;
        });

        await _activityEngine.Dispatch(startAnalysisActivity, cancellationToken);
        await _activityEngine.Wait(startAnalysisActivity, cancellationToken);

        return exitStatus;
    }
}
