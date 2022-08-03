using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions.Git;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class GitCloneCommandRunner : CommandRunner<GitCloneCommand, GitCloneCommandOptions>
{
    private ICacheManager CacheManager { get; }

    public GitCloneCommandRunner(IServiceProvider serviceProvider, ICacheManager cacheManager, Runner runner)
        : base(serviceProvider, runner)
    {
        CacheManager = cacheManager;
    }

    public override int Run(GitCloneCommandOptions options, InvocationContext context)
    {
        // Clone or pull the given repository and branch.
        var gitRepository = new GitSource(options.RepoUrl, options.Branch, options.CacheDir, CacheManager);
        try
        {
            gitRepository.CloneOrPull(options.GitPath);
        }
        catch (GitException e)
        {
            context.Console.Error.WriteLine(e.Message);
            return false.ToExitCode();
        }

        // Output the hash to the command line for use by the caller.
        context.Console.Out.WriteLine(gitRepository.Hash);
        return true.ToExitCode();
    }
}
