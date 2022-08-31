using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions.Git;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class GitCloneCommandRunner : CommandRunner<GitCloneCommand, GitCloneCommandOptions>
{
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly IApplicationEventEngine _eventEngine;

    private readonly ICachedGitSourceRepository _gitSourceRepository;

    public GitCloneCommandRunner(IServiceProvider serviceProvider, Runner runner,
        ICachedGitSourceRepository gitSourceRepository,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine)
        : base(serviceProvider, runner)
    {
        _gitSourceRepository = gitSourceRepository;
        _activityEngine = activityEngine;
        _eventEngine = eventEngine;
    }

    public override int Run(GitCloneCommandOptions options, InvocationContext context)
    {
        _activityEngine.Dispatch(new CloneGitRepositoryActivity(_gitSourceRepository,
            options.RepoUrl, options.Branch, options.CacheDir, options.GitPath));

        var exitCode = true.ToExitCode();
        _eventEngine.On<GitRepositoryClonedEvent>(clonedEvent =>
        {
            // Output the id to the command line for use by the caller.
            context.Console.Out.WriteLine(clonedEvent.GitRepositoryId);
        });

        _eventEngine.On<CloneGitRepositoryFailedEvent>(clonedEvent =>
        {
            context.Console.Error.WriteLine(clonedEvent.ErrorMessage);
            exitCode = false.ToExitCode();
        });

        _activityEngine.Wait();
        return exitCode;
    }
}
