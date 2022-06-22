using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Environment = System.Environment;

namespace Corgibytes.Freshli.Cli.IoC;

public class FreshliServiceBuilder
{
    public IServiceCollection Services { get; }

    public FreshliServiceBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public void Register()
    {
        Services.AddSingleton<IEnvironment, Functionality.Environment>();
        RegisterBaseCommand();
        RegisterScanCommand();
        RegisterCacheCommand();
        RegisterAgentsCommand();
    }

    public void RegisterBaseCommand()
    {
        Services.AddScoped<Runner>();
    }

    public void RegisterScanCommand()
    {
        Services.AddScoped<ICommandRunner<ScanCommand, ScanCommandOptions>, ScanCommandRunner>();
        Services.AddNamedScoped<IOutputFormatter, JsonOutputFormatter>(FormatType.Json);
        Services.AddNamedScoped<IOutputFormatter, CsvOutputFormatter>(FormatType.Csv);
        Services.AddNamedScoped<IOutputFormatter, YamlOutputFormatter>(FormatType.Yaml);
        Services.AddNamedScoped<IOutputStrategy, FileOutputStrategy>(OutputStrategyType.File);
        Services.AddNamedScoped<IOutputStrategy, ConsoleOutputStrategy>(OutputStrategyType.Console);
        Services.AddOptions<ScanCommandOptions>().BindCommandLine();
    }

    public void RegisterCacheCommand()
    {
        Services.AddScoped<ICommandRunner<CacheCommand, CacheCommandOptions>, CacheCommandRunner>();
        Services.AddOptions<CacheCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CacheCommand, CachePrepareCommandOptions>, CachePrepareCommandRunner>();
        Services.AddOptions<CachePrepareCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CacheCommand, CacheDestroyCommandOptions>, CacheDestroyCommandRunner>();
        Services.AddOptions<CacheDestroyCommandOptions>().BindCommandLine();
    }

    public void RegisterAgentsCommand()
    {
        Services.AddTransient<AgentsDetector>();

        Services.AddScoped<ICommandRunner<AgentsCommand, EmptyCommandOptions>, AgentsCommandRunner>();
        Services.AddOptions<EmptyCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<AgentsDetectCommand, EmptyCommandOptions>, AgentsDetectCommandRunner>();
        Services.AddOptions<EmptyCommandOptions>().BindCommandLine();
    }
}
