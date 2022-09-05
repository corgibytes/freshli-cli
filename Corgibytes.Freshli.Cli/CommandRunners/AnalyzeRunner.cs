using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AnalyzeRunner : CommandRunner<AnalyzeCommand, AnalyzeCommandOptions>
{
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly ICacheManager _cacheManager;

    public AnalyzeRunner(IServiceProvider serviceProvider, Runner runner, IApplicationActivityEngine activityEngine, ICacheManager cacheManager) : base(serviceProvider, runner)
    {
        _activityEngine = activityEngine;
        _cacheManager = cacheManager;
    }

    public override int Run(AnalyzeCommandOptions options, InvocationContext context)
    {
        _activityEngine.Dispatch(new StartAnalysisActivity(_cacheManager, new HistoryIntervalParser())
        {
            CacheDirectory = options.CacheDir,
            HistoryInterval = options.HistoryInterval,
            RepositoryBranch = options.Branch,
            RepositoryUrl = options.RepositoryLocation,
            UseCommitHistory = options.CommitHistory
        });

        // @TODO
        // Return URL after submitting it to Freshli

        context.Console.Out.WriteLine("https://freshli.app/");

        return 0;
    }
}

