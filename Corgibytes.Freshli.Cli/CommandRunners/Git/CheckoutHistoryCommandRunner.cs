using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners.Git;

public class CheckoutHistoryCommandRunner : CommandRunner<CheckoutHistoryCommandOptions>
{
    private readonly GitArchive _gitArchive;

    public CheckoutHistoryCommandRunner(IServiceProvider serviceProvider, Runner runner, GitArchive gitArchive) : base(serviceProvider, runner)
    {
        _gitArchive = gitArchive;
    }

    public override int Run(CheckoutHistoryCommandOptions options, InvocationContext context)
    {
        context.Console.Out.WriteLine(
            _gitArchive.CreateArchive(
                options.RepositoryId,
                options.CacheDir,
                new(options.Sha),
                options.GitPath
            )
        );

        return 0;
    }
}

