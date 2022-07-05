using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class GitCommandRunner : CommandRunner<GitCommandOptions>
{
    public GitCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {
    }

    public override int Run(GitCommandOptions options, InvocationContext context) => 0;
}

public class GitCloneCommandRunner : CommandRunner<GitCloneCommandOptions>
{
    public GitCloneCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {
    }

    public override int Run(GitCloneCommandOptions options, InvocationContext context)
    {
        // Clone or pull the given repository and branch.
        var gitRepository = new GitRepository(options.RepoUrl, options.Branch, options.CacheDir);
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
