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
    private readonly ICacheManager _cacheManager;
    private readonly IApplicationEventEngine _eventEngine;
    private readonly IResultsApi _resultsApi;

    public AnalyzeRunner(
        IServiceProvider serviceProvider, Runner runner, IApplicationActivityEngine activityEngine,
        ICacheManager cacheManager, IApplicationEventEngine eventEngine, IResultsApi resultsApi
    ) : base(serviceProvider, runner)
    {
        _activityEngine = activityEngine;
        _cacheManager = cacheManager;
        _eventEngine = eventEngine;
        _resultsApi = resultsApi;
    }

    public override int Run(AnalyzeCommandOptions options, InvocationContext context)
    {
        _activityEngine.Dispatch(new StartAnalysisActivity(_cacheManager, new HistoryIntervalParser())
        {
            CacheDirectory = options.CacheDir,
            HistoryInterval = options.HistoryInterval,
            RepositoryBranch = options.Branch,
            RepositoryUrl = options.RepositoryLocation,
            UseCommitHistory = options.CommitHistory,
            GitPath = options.GitPath
        });

        _eventEngine.On<AnalysisStartedEvent>(startEvent =>
        {
            context.Console.Out.WriteLine("Results will be available at: " + _resultsApi.GetResultsUrl(startEvent.AnalysisId));
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

        return 0;
    }
}
