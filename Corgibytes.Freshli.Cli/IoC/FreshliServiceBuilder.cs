using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.CommandRunners.Git;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Commands.Git;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC;

public class FreshliServiceBuilder
{
    public FreshliServiceBuilder(IServiceCollection services) => Services = services;

    private IServiceCollection Services { get; }

    public void Register()
    {
        Services.AddSingleton<IEnvironment, Environment>();
        RegisterBaseCommand();
        RegisterScanCommand();
        RegisterCacheCommand();
        RegisterAgentsCommand();
        RegisterGitCommand();
    }

    private void RegisterBaseCommand() => Services.AddScoped<Runner>();

    private void RegisterScanCommand()
    {
        Services.AddScoped<ICommandRunner<ScanCommand, ScanCommandOptions>, ScanCommandRunner>();
        Services.AddNamedScoped<IOutputFormatter, JsonOutputFormatter>(FormatType.Json);
        Services.AddNamedScoped<IOutputFormatter, CsvOutputFormatter>(FormatType.Csv);
        Services.AddNamedScoped<IOutputFormatter, YamlOutputFormatter>(FormatType.Yaml);
        Services.AddNamedScoped<IOutputStrategy, FileOutputStrategy>(OutputStrategyType.File);
        Services.AddNamedScoped<IOutputStrategy, ConsoleOutputStrategy>(OutputStrategyType.Console);
        Services.AddOptions<ScanCommandOptions>().BindCommandLine();
    }

    private void RegisterCacheCommand()
    {
        Services.AddScoped<ICommandRunner<CacheCommand, CacheCommandOptions>, CacheCommandRunner>();
        Services.AddOptions<CacheCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CacheCommand, CachePrepareCommandOptions>, CachePrepareCommandRunner>();
        Services.AddOptions<CachePrepareCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CacheCommand, CacheDestroyCommandOptions>, CacheDestroyCommandRunner>();
        Services.AddOptions<CacheDestroyCommandOptions>().BindCommandLine();
    }

    private void RegisterAgentsCommand()
    {
        Services.AddTransient<AgentsDetector>();

        Services.AddScoped<ICommandRunner<AgentsCommand, EmptyCommandOptions>, AgentsCommandRunner>();
        Services.AddOptions<EmptyCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<AgentsDetectCommand, EmptyCommandOptions>, AgentsDetectCommandRunner>();
        Services.AddOptions<EmptyCommandOptions>().BindCommandLine();
    }

    private void RegisterGitCommand()
    {
        Services.AddScoped<ICommandRunner<GitCommand, GitCommandOptions>, GitCommandRunner>();
        Services.AddOptions<GitCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<GitCloneCommand, GitCloneCommandOptions>, GitCloneCommandRunner>();
        Services.AddOptions<GitCloneCommandOptions>().BindCommandLine();

        Services
            .AddScoped<ICommandRunner<ComputeHistoryCommand, ComputeHistoryCommandOptions>,
                ComputeHistoryCommandRunner>();
        Services.AddOptions<ComputeHistoryCommandOptions>().BindCommandLine();
    }
}
