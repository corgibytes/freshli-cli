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
    private readonly IConfiguration _configuration;
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly ICacheManager _cacheManager;
    private readonly IApplicationEventEngine _eventEngine;
    private readonly IResultsApi _resultsApi;

    public AnalyzeRunner(
        IServiceProvider serviceProvider, Runner runner, IConfiguration configuration, IApplicationActivityEngine activityEngine,
        ICacheManager cacheManager, IApplicationEventEngine eventEngine, IResultsApi resultsApi
    ) : base(serviceProvider, runner)
    {
        _configuration = configuration;
        _activityEngine = activityEngine;
        _cacheManager = cacheManager;
        _eventEngine = eventEngine;
        _resultsApi = resultsApi;
    }

    public override int Run(AnalyzeCommandOptions options, InvocationContext context)
    {
        _configuration.CacheDir = options.CacheDir;
        _configuration.GitPath = options.GitPath;

        _activityEngine.Dispatch(new StartAnalysisActivity(_configuration, _cacheManager, new HistoryIntervalParser())
        {
            HistoryInterval = options.HistoryInterval,
            RepositoryBranch = options.Branch,
            RepositoryUrl = options.RepositoryLocation,
            UseCommitHistory = options.CommitHistory ? CommitHistory.Full : CommitHistory.AtInterval,
        });

        var exitStatus = 0;

        _eventEngine.On<AnalysisFailureLoggedEvent>(analysisFailure =>
        {
            context.Console.Out.WriteLine("Analysis failed because: " + analysisFailure.ErrorEvent.ErrorMessage);
            exitStatus = 1;
        });

        _eventEngine.On<AnalysisStartedEvent>(startEvent =>
        {
            context.Console.Out.WriteLine("Results will be available at: " +
                                          _resultsApi.GetResultsUrl(startEvent.AnalysisId));
        });

        _eventEngine.On<LibYearComputedEvent>(computedEvent =>
        {
            var libYearSummed = 0.0;
            if (computedEvent.LibYearPackages != null)
            {
                libYearSummed = computedEvent.LibYearPackages.Sum(libYear => libYear.LibYear);
            }

            context.Console.Out.WriteLine($"Libyear at {computedEvent.AnalysisLocation?.CommitId} is {libYearSummed}");
        });

        _activityEngine.Wait();

        return exitStatus;
    }
}
