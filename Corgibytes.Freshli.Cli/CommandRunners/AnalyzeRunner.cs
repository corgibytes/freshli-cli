using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Linq;
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
        IServiceProvider serviceProvider, Runner runner, IConfiguration configuration,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine,
        IResultsApi resultsApi
    ) : base(serviceProvider, runner)
    {
        _configuration = configuration;
        _activityEngine = activityEngine;
        _eventEngine = eventEngine;
        _resultsApi = resultsApi;
    }

    public override int Run(AnalyzeCommandOptions options, InvocationContext context)
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
            context.Console.Out.WriteLine("Analysis failed because: " + analysisFailure.ErrorEvent.ErrorMessage);
            exitStatus = 1;
        });

        _eventEngine.On<AnalysisStartedEvent>(startEvent =>
        {
            context.Console.Out.WriteLine(
                "Results will be available at: " +
                _resultsApi.GetResultsUrl(startEvent.AnalysisId)
            );
        });

        _activityEngine.Wait();

        return exitStatus;
    }
}
