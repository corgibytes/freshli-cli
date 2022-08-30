using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions.Git;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class CheckoutHistoryCommandRunner : CommandRunner<CheckoutHistoryCommand, CheckoutHistoryCommandOptions>
{
    private readonly IGitManager _gitManager;
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly IApplicationEventEngine _eventEngine;

    public CheckoutHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner, IGitManager gitManager, IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine) :
        base(serviceProvider, runner)
    {
        _gitManager = gitManager;
        _activityEngine = activityEngine;
        _eventEngine = eventEngine;
    }

    public override int Run(CheckoutHistoryCommandOptions options, InvocationContext context)
    {
        _activityEngine.Dispatch(
            new CheckoutHistoryActivity(
                _gitManager, options.GitPath, options.CacheDir, options.RepositoryId, options.Sha));

        _eventEngine.On<HistoryStopCheckedOutEvent>(historyEvent =>
            context.Console.Out.WriteLine(historyEvent.AnalysisLocation.Path));

        _activityEngine.Wait();

        return 0;
    }
}
