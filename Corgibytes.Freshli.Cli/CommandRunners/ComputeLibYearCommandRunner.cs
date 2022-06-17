using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class ComputeLibYearCommandRunner : CommandRunner<ComputeLibYearCommandOptions>
{
    public ComputeLibYearCommandRunner(IServiceProvider serviceProvider, Runner runner) : base(serviceProvider, runner)
    {

    }

    public override int Run(ComputeLibYearCommandOptions options, InvocationContext context)
    {
        if (string.IsNullOrWhiteSpace(options.FilePath?.FullName))
        {
            throw new ArgumentNullException(nameof(options), CliOutput.ComputeLibYearCommandRunner_Run_FilePath_should_not_be_null_or_empty);
        }

        var calculateLibYearFromCycloneDxFile = Services.GetService<CalculateLibYearFromCycloneDxFile>();
        if (calculateLibYearFromCycloneDxFile == null)
        {
            throw new NullReferenceException("Could not get service CalculateLibYearFromCycloneDxFile. Forgot to wire it?");
        }

        context.Console.Out.Write(calculateLibYearFromCycloneDxFile.AsDecimalNumber(options.FilePath.ToString()).ToString());

        return 0;
    }
}

