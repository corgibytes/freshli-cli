using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands.Scan.Formatters;
using Corgibytes.Freshli.Cli.Commands.Scan.OutputStrategies;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Scan;

public class ScanCommandRunner : CommandRunner<ScanCommand, ScanCommandOptions>
{
    public ScanCommandRunner(IServiceProvider serviceProvider, IRunner runner) : base(serviceProvider, runner)
    {
    }

    public override ValueTask<int> Run(ScanCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.Path.FullName))
        {
            throw new ArgumentNullException(nameof(options),
                CliOutput.ScanCommandRunner_Run_Path_should_not_be_null_or_empty);
        }

        var formatter = options.Format.ToFormatter(Services);
        var outputStrategies = options.Output.ToOutputStrategies(Services);

        var results = Runner.Run(options.Path.FullName);

        foreach (var output in outputStrategies)
        {
            output.Send(results, formatter, options);
        }

        return ValueTask.FromResult(0);
    }
}
