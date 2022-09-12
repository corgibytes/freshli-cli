using System.Collections.Generic;
using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.IoC.Engine;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Lib;
using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Corgibytes.Freshli.Cli.IoC;

public class FreshliServiceBuilder
{
    public FreshliServiceBuilder(IServiceCollection services) => Services = services;

    private IServiceCollection Services { get; }

    public void Register()
    {
        Services.AddSingleton<IEnvironment, Environment>();
        Services.AddSingleton<ICacheManager, CacheManager>();
        Services.AddSingleton<IAgentManager, AgentManager>();
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
        Services.AddScoped<ICommandRunner<AnalyzeCommand, AnalyzeCommandOptions>, AnalyzeRunner>();
        Services.AddScoped<IResultsApi, ResultsApi>();
        Services.AddScoped<IHistoryIntervalParser, HistoryIntervalParser>();
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

        Services.AddSingleton<IGitManager, GitManager>();
        Services.AddSingleton<GitArchive>();
        Services.AddSingleton<ICachedGitSourceRepository, CachedGitSourceRepository>();
    }

    private void RegisterComputeLibYearCommand()
    {
        Services.AddScoped<ICalculateLibYearFromFile, CalculateLibYearFromCycloneDxFile>();
        Services.AddTransient<ReadCycloneDxFile>();
        Services.AddScoped<IFileReader, CycloneDxFileReaderFromFileReaderSystem>();

        Services.AddTransient<IDependencyManagerRepository, AgentsRepository>();
        Services.AddTransient<IAgentReader, AgentReader>();
    }

    // Based on https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L43
    // and https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L168
    private void RegisterApplicationEngine()
    {
        Services.AddSingleton<ApplicationEngine>();
        Services.AddSingleton<IApplicationActivityEngine, ApplicationEngine>();
        Services.AddSingleton<IApplicationEventEngine, ApplicationEngine>();

        RegisterHangfire();
        RegisterHangfireConfiguration();
        RegisterHangfireServer();
    }

    // Based on https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L180
    private void RegisterHangfireServer()
    {
        Services.AddSingleton(new BackgroundJobServerOptions { WorkerCount = 10 });

        Services.AddTransient<IHostedService, BackgroundJobServerHostedService>(provider =>
        {
            var options = provider.GetService<BackgroundJobServerOptions>() ?? new BackgroundJobServerOptions();
            var storage = provider.GetService<JobStorage>() ?? JobStorage.Current;
            return new BackgroundJobServerHostedService(storage, options, new List<IBackgroundProcess>());
        });
    }

    // Based on https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L43
    private void RegisterHangfire()
    {
        JobStorage.Current = new MemoryStorage();

        Services.AddSingleton<IContractResolver, JsonContractResolver>();

        Services.TryAddSingletonChecked(_ => JobStorage.Current);
        Services.TryAddSingletonChecked(_ => JobActivator.Current);

        Services.TryAddSingleton<IJobFilterProvider>(_ => JobFilterProviders.Providers);
        Services.TryAddSingleton<ITimeZoneResolver>(_ => new DefaultTimeZoneResolver());

        Services.TryAddSingleton<IJobFilterProvider>(_ => JobFilterProviders.Providers);
        Services.TryAddSingleton<ITimeZoneResolver>(_ => new DefaultTimeZoneResolver());

        Services.TryAddSingleton(x => new DefaultClientManagerFactory(x));
        Services.TryAddSingletonChecked<IBackgroundJobClientFactory>(x => x.GetService<DefaultClientManagerFactory>()!);
        Services.TryAddSingletonChecked<IRecurringJobManagerFactory>(x => x.GetService<DefaultClientManagerFactory>()!);

        Services.TryAddSingletonChecked(x => x
            .GetService<IBackgroundJobClientFactory>()!.GetClient(x.GetService<JobStorage>()!));

        Services.TryAddSingletonChecked(x => x
            .GetService<IRecurringJobManagerFactory>()!.GetManager(x.GetService<JobStorage>()!));
    }

    // Based on https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L76
    private void RegisterHangfireConfiguration() =>
        Services.AddSingleton(serviceProvider =>
        {
            var configurationInstance = GlobalConfiguration.Configuration;

            // init defaults for log provider and job activator
            // they may be overwritten by the configuration callback later

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                configurationInstance.UseLogProvider(new MicrosoftExtensionsCoreLogProvider(loggerFactory));
            }

            var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
            if (scopeFactory != null)
            {
                configurationInstance.UseActivator(new MicrosoftExtensionsJobActivator(scopeFactory));
            }

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = serviceProvider.GetRequiredService<IContractResolver>(),
                TypeNameHandling = TypeNameHandling.All
            };
            configurationInstance.UseSerializerSettings(jsonSettings);

            configurationInstance.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });

            return configurationInstance;
        });
}
