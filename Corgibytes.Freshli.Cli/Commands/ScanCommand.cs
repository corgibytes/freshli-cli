using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands;

public class ScanCommand : RunnableCommand<ScanCommand, ScanCommandOptions>
{
    public ScanCommand() : base("scan", "Scan command returns metrics results for given local repository path")
    {
        Option<FormatType> formatOption = new(new[] { "--format", "-f" },
            description: "Represents the output format type - It's value is case insensitive",
            getDefaultValue: () => FormatType.Json)
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };

        Option<IEnumerable<OutputStrategyType>> outputOption = new(new[] { "--output", "-o" },
            description:
            "Represents where you want to output the result. This option is case sensitive and you can specify more than one by including it multiple times. Allowed values are [ console | file ]",
            getDefaultValue: () => new List<OutputStrategyType> { OutputStrategyType.Console })
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        AddOption(formatOption);
        AddOption(outputOption);

        Argument<DirectoryInfo> pathArgument = new("path", "Source code repository path")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        AddArgument(pathArgument);
    }


    protected override int Run(IHost host, InvocationContext context, ScanCommandOptions options)
    {
        context.Console.Out.Write($"{CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler}\n");
        return base.Run(host, context, options ?? throw new ArgumentNullException(nameof(options)));
    }
}
