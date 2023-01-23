using System.CommandLine.Hosting;
using System.Net.Http;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC;

public class FreshliServiceBuilder
{
    public FreshliServiceBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    private IServiceCollection Services { get; }
    private IConfiguration Configuration { get; }

    public void Register()
    {
        Services.AddSingleton(Configuration);
        Services.AddSingleton(AnsiConsole.Create(new AnsiConsoleSettings()));
        Services.AddSingleton<IEnvironment, Environment>();
        Services.AddScoped<IExecutableFinder, ExecutableFinder>();
        Services.AddSingleton<ICacheManager, CacheManager>();
        Services.AddSingleton<IAgentManager, AgentManager>();
        Services.AddScoped<IHistoryIntervalParser, HistoryIntervalParser>();
        Services.AddScoped<IRunner, Runner>();
        Services.AddSingleton<HttpClient>();
        RegisterBaseCommand();
        RegisterAnalyzeCommand();
        RegisterFailCommand();
        RegisterLoadServiceCommand();
        RegisterScanCommand();
        RegisterCacheCommand();
        RegisterAgentsCommand();
        RegisterGitCommand();
        RegisterComputeLibYearCommand();
        RegisterApplicationEngine();
    }

    private void RegisterBaseCommand() => Services.AddScoped<Runner>();

    private void RegisterFailCommand() =>
        Services.AddScoped<ICommandRunner<FailCommand, EmptyCommandOptions>, FailCommandRunner>();

    private void RegisterAnalyzeCommand()
    {
        Services.AddScoped<IAnalyzeProgressReporter, PlainTextAnalyzeProgressReporter>();
        Services.AddScoped<ICommandRunner<AnalyzeCommand, AnalyzeCommandOptions>, AnalyzeRunner>();
        Services.AddScoped<IResultsApi, ResultsApi>();
        Services.AddScoped<IHistoryIntervalParser, HistoryIntervalParser>();
        Services.AddScoped<IBomReader, CycloneDxBomReader>();
        Services.AddScoped<IPackageLibYearCalculator, PackageLibYearCalculator>();
    }

    private void RegisterLoadServiceCommand() =>
        Services.AddScoped<ICommandRunner<LoadServiceCommand, EmptyCommandOptions>, LoadServiceCommandRunner>();

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
        Services.AddScoped<IAgentsDetector, AgentsDetector>();

        Services.AddScoped<ICommandRunner<AgentsCommand, EmptyCommandOptions>, AgentsCommandRunner>();
        Services.AddOptions<EmptyCommandOptions>().BindCommandLine();

        Services.AddScoped<ICommandRunner<AgentsDetectCommand, EmptyCommandOptions>, AgentsDetectCommandRunner>();
        Services.AddOptions<EmptyCommandOptions>().BindCommandLine();

        Services.AddTransient<AgentsVerifier>();

        Services
            .AddScoped<ICommandRunner<AgentsVerifyCommand, AgentsVerifyCommandOptions>, AgentsVerifyCommandRunner>();
        Services.AddOptions<AgentsVerifyCommandOptions>().BindCommandLine();
    }

    private void RegisterGitCommand()
    {
        Services.AddScoped<IComputeHistory, ComputeHistory>();
        Services.AddScoped<IListCommits, ListCommits>();

        Services.AddScoped<IGitManager, GitManager>();
        Services.AddScoped<GitArchive>();
        Services.AddScoped<ICachedGitSourceRepository, CachedGitSourceRepository>();
    }

    private void RegisterComputeLibYearCommand()
    {
        Services.AddTransient<CycloneDxBomReader>();
        Services.AddScoped<IFileReader, CycloneDxFileReaderFromFileReaderSystem>();
    }

    private void RegisterApplicationEngine()
    {
        Services.AddSingleton<ApplicationEngine>();
        Services.AddSingleton<IApplicationActivityEngine, ApplicationEngine>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationEngine>());
        Services.AddSingleton<IApplicationEventEngine, ApplicationEngine>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationEngine>());
        Services.AddSingleton<ICommandInvoker, CommandInvoker>();

        Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
    }
}
