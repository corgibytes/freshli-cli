using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands;

public class ScanCommand : RunnableCommand<ScanCommandOptions>
{
    public ScanCommand() : base("scan", $"{CliOutput.Help_ScanCommand_Description}")
    {

        Option<FormatType> formatOption = new(new[] {"--format", "-f"},
            description: $"{CliOutput.Help_ScanCommand_Option_Format}",
            getDefaultValue: () => FormatType.Json)
        {
            AllowMultipleArgumentsPerToken = false, Arity = ArgumentArity.ExactlyOne,
        };

        Option<IEnumerable<OutputStrategyType>> outputOption = new(new[] {"--output", "-o"},
            description:
            $"{CliOutput.Help_ScanCommand_Option_Output} [ console | file ]",
            getDefaultValue: () => new List<OutputStrategyType>() {OutputStrategyType.Console})
        {
            AllowMultipleArgumentsPerToken = true, Arity = ArgumentArity.OneOrMore,
        };

        AddOption(formatOption);
        AddOption(outputOption);

        Argument<DirectoryInfo> pathArgument = new("path", $"{CliOutput.Help_ScanCommand_Argument_Path}")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        AddArgument(pathArgument);
    }

    protected override int Run(IHost host, InvocationContext context, ScanCommandOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        context.Console.Out.WriteLine($"{CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler}");

        return base.Run(host, context, options);
    }
}
