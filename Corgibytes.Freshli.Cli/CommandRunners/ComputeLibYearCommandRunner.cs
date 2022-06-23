using System;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class ComputeLibYearCommandRunner : CommandRunner<ComputeLibYearCommandOptions>
{
    private readonly CalculateLibYearFromCycloneDxFile _calculateLibYearFromCycloneDxFile;

    public ComputeLibYearCommandRunner(IServiceProvider serviceProvider, Runner runner, CalculateLibYearFromCycloneDxFile calculateLibYearFromCycloneDxFile) : base(serviceProvider, runner)
    {
        _calculateLibYearFromCycloneDxFile = calculateLibYearFromCycloneDxFile;
    }

    public override int Run(ComputeLibYearCommandOptions options, InvocationContext context)
    {
        if (string.IsNullOrWhiteSpace(options.FilePath?.FullName))
        {
            throw new ArgumentNullException(nameof(options), CliOutput.ComputeLibYearCommandRunner_Run_FilePath_should_not_be_null_or_empty);
        }

        context.Console.Out.Write(_calculateLibYearFromCycloneDxFile.TotalAsDecimalNumber(options.FilePath.ToString()).ToString());

        return 0;
    }
}

