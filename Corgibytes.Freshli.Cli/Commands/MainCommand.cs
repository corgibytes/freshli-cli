using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands.Agents;
using Corgibytes.Freshli.Cli.Commands.Analyze;
using Corgibytes.Freshli.Cli.Commands.Auth;
using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Commands.Fail;
using Corgibytes.Freshli.Cli.Commands.LoadService;
using Corgibytes.Freshli.Cli.Commands.Scan;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands;

public class MainCommand : RootCommand
{
    public MainCommand(IConfiguration configuration) : base(CliOutput.Help_MainCommand_Description)
    {
        var cacheDirOption = new Option<string>(
            new[] { "--cache-dir" },
            description: CliOutput.Help_Option_CacheDir_Description,
            getDefaultValue: () => configuration.CacheDir)
        { Arity = ArgumentArity.ExactlyOne };
        AddGlobalOption(cacheDirOption);

        var logLevelOption = new Option<string>("--loglevel", description: "the minimum log level to log to console",
            getDefaultValue: () => "Warn");
        AddOption(logLevelOption);

        var logFileOption = new Option<string>("--logfile", "file for logs instead of logging to console");
        AddOption(logFileOption);

        var workers = new Option<int>("--workers",
            "The number of worker processes that should be running at any given time. This defaults to twice the number of CPU cores.")
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(workers);

        // Add commands here!
        Add(new AgentsCommand());
        Add(new AnalyzeCommand(configuration));
        Add(new AuthCommand());
        Add(new CacheCommand());
        Add(new ScanCommand());

        if (ShouldIncludeFailCommand)
        {
            Add(new FailCommand());
        }

        if (ShouldIncludeLoadServiceCommand)
        {
            Add(new LoadServiceCommand());
        }
    }

    public static bool ShouldIncludeFailCommand { get; set; }
    public static bool ShouldIncludeLoadServiceCommand { get; set; }
}
