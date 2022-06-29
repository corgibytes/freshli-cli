using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.CommandRunners.Git;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

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
        RegisterBaseCommand();
        RegisterScanCommand();
        RegisterCacheCommand();
        RegisterCheckoutHistoryCommand();
    }

    public void RegisterBaseCommand()
    {
        Services.AddScoped<Runner>();
    }

    public void RegisterScanCommand()
    {
        Services.AddScoped<ICommandRunner<ScanCommandOptions>, ScanCommandRunner>();
        Services.AddNamedScoped<IOutputFormatter, JsonOutputFormatter>(FormatType.Json);
        Services.AddNamedScoped<IOutputFormatter, CsvOutputFormatter>(FormatType.Csv);
        Services.AddNamedScoped<IOutputFormatter, YamlOutputFormatter>(FormatType.Yaml);
        Services.AddNamedScoped<IOutputStrategy, FileOutputStrategy>(OutputStrategyType.File);
        Services.AddNamedScoped<IOutputStrategy, ConsoleOutputStrategy>(OutputStrategyType.Console);
        Services.AddOptions<ScanCommandOptions>().BindCommandLine();
    }

    public void RegisterCacheCommand()
    {
        Services.AddScoped<ICommandRunner<CacheCommandOptions>, CacheCommandRunner>();
        Services.AddOptions<CacheCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CachePrepareCommandOptions>, CachePrepareCommandRunner>();
        Services.AddOptions<CachePrepareCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CacheDestroyCommandOptions>, CacheDestroyCommandRunner>();
        Services.AddOptions<CacheDestroyCommandOptions>().BindCommandLine();
    }

    public void RegisterCheckoutHistoryCommand()
    {
        Services.AddScoped<ICommandRunner<GitCommandOptions>, GitCommandRunner>();
        Services.AddOptions<GitCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<CheckoutHistoryCommandOptions>, CheckoutHistoryCommandRunner>();
        Services.AddOptions<CheckoutHistoryCommandOptions>().BindCommandLine();
    }
}
