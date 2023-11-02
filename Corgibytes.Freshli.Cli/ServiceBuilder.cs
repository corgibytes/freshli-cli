using System.CommandLine.Hosting;
using System.Net.Http;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Commands.Agents;
using Corgibytes.Freshli.Cli.Commands.Analyze;
using Corgibytes.Freshli.Cli.Commands.Auth;
using Corgibytes.Freshli.Cli.Commands.Cache;
using Corgibytes.Freshli.Cli.Commands.Fail;
using Corgibytes.Freshli.Cli.Commands.LoadService;
using Corgibytes.Freshli.Cli.Commands.Scan;
using Corgibytes.Freshli.Cli.Commands.Scan.Formatters;
using Corgibytes.Freshli.Cli.Commands.Scan.OutputStrategies;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Api.Auth;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Corgibytes.Freshli.Cli;

public class ServiceBuilder
{
    public ServiceBuilder(IServiceCollection services, IConfiguration configuration)
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
        Services.AddSingleton<IFileValidator, FileValidator>();
        Services.AddSingleton<IBillOfMaterialsProcessor, BillOfMaterialsProcessor>();
        Services.AddSingleton<IAuthClient, AuthClient>();
        RegisterBaseCommand();
        RegisterAnalyzeCommand();
        RegisterAuthCommand();
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
        Services.AddScoped<IAnalyzeProgressReporter, SpectreConsoleAnalyzeProgressReporter>();
        Services.AddScoped<ICommandRunner<AnalyzeCommand, AnalyzeCommandOptions>, AnalyzeRunner>();
        Services.AddScoped<IHistoryIntervalParser, HistoryIntervalParser>();
        Services.AddScoped<IBomReader, CycloneDxBomReader>();
    }

    private void RegisterAuthCommand()
    {
        Services.AddScoped<ICommandRunner<AuthCommand, EmptyCommandOptions>, AuthCommandRunner>();
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
