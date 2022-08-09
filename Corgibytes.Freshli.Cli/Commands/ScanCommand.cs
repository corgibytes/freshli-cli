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

public class ScanCommand : RunnableCommand<ScanCommand, ScanCommandOptions>
{
    public ScanCommand() : base("scan", CliOutput.Help_ScanCommand_Description)
    {
        var formatOption = new Option<FormatType>(new[] { "--format", "-f" },
            description: CliOutput.Help_ScanCommand_Option_Format, getDefaultValue: () => FormatType.Json)
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };

        var outputOption =
            new Option<IEnumerable<OutputStrategyType>>(new[] { "--output", "-o" },
                description: $"{CliOutput.Help_ScanCommand_Option_Output} [ console | file ]",
                getDefaultValue: () => new List<OutputStrategyType> { OutputStrategyType.Console })
            {
                AllowMultipleArgumentsPerToken = true,
                Arity = ArgumentArity.OneOrMore
            };

        AddOption(formatOption);
        AddOption(outputOption);

        var pathArgument =
            new Argument<DirectoryInfo>("path", CliOutput.Help_ScanCommand_Argument_Path)
            {
                Arity = ArgumentArity.ExactlyOne
            };

        AddArgument(pathArgument);
    }


    protected override int Run(IHost host, InvocationContext context, ScanCommandOptions options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        context.Console.Out.WriteLine(CliOutput.ScanCommand_ScanCommand_Executing_scan_command_handler);
        return base.Run(host, context, options);
    }
}
