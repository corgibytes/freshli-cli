using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class GitCommandRunner : CommandRunner<GitCommandOptions>
{
    public GitCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    { }

    public override int Run(GitCommandOptions options, InvocationContext context)
    {
        return 0;
    }
}

public class GitCloneCommandRunner : CommandRunner<GitCloneCommandOptions>
{
    public GitCloneCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    { }

    public override int Run(GitCloneCommandOptions options, InvocationContext context)
    {
        // IMPLEMENTATION HERE
        // 1. Create/verify ["repositories"] directory in cache dir
        // 2. Generate unique ID for repository.
        // 3. Store ID, URL, and folder path in cache DB
        // 4. Clone repository from options.RepoUrl
        // 5. If options.Branch is defined, checkout branch
        return true.ToExitCode();
    }
}
