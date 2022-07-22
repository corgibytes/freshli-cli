using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands;

public class ComputeLibYearCommand : RunnableCommand<ComputeLibYearCommand, ComputeLibYearCommandOptions>
{
    public ComputeLibYearCommand() : base("compute-libyear", "Computes the libyear for a given CycloneDX file")
    {
        Argument<FileInfo> filepathArgument = new Argument<FileInfo>("file-path", "Filepath of an CycloneDX file")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        AddArgument(filepathArgument);
    }

    protected override int Run(IHost host, InvocationContext context, ComputeLibYearCommandOptions options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));

        return base.Run(host, context, options);
    }
}
