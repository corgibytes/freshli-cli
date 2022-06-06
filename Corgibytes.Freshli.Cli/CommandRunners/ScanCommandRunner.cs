using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class ScanCommandRunner : CommandRunner<ScanCommandOptions>
{

    public ScanCommandRunner(IServiceProvider serviceProvider, Runner runner): base(serviceProvider,runner)
    {

    }

    public override int Run(ScanCommandOptions options, InvocationContext context)
    {
        if (string.IsNullOrWhiteSpace(options.Path?.FullName))
        {
            throw new ArgumentNullException(nameof(options), CliOutput.ScanCommandRunner_Run_Path_should_not_be_null_or_empty);
        }

        IOutputFormatter formatter = options.Format.ToFormatter(Services);
        IEnumerable<IOutputStrategy> outputStrategies = options.Output.ToOutputStrategies(Services);

        IList<ScanResult> results = Runner.Run(options.Path.FullName);

        foreach (IOutputStrategy output in outputStrategies)
        {
            output.Send(results, formatter, options);
        }

        return 0;
    }
}
