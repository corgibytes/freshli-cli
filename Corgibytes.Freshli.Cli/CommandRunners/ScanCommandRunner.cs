using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners
{

    public class ScanCommandRunner : ICommandRunner<ScanCommandOptions>
    {
        public Runner Runner { get; set; }
        private readonly IServiceProvider _services;

        public ScanCommandRunner(IServiceProvider serviceProvider, Runner runner)
        {
            Runner = runner;
            _services = serviceProvider;
        }

        public virtual int Run(ScanCommandOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Path))
            {
                throw new ArgumentNullException(nameof(options), CliOutput.ScanCommandRunner_Run_Path_should_not_be_null_or_empty);
            }

            IOutputFormatter formatter = options.Format.ToFormatter(_services);
            IEnumerable<IOutputStrategy> outputStrategies = options.Output.ToOutputStrategies(_services);

            IList<MetricsResult> results = Runner.Run(options.Path);

            foreach (IOutputStrategy output in outputStrategies)
            {
                output.Send(results, formatter, options);
            }

            return 0;
        }
    }
}
