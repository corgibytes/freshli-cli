using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC
{
    public class FreshliServiceBuilder
    {
        public IServiceCollection Services { get; }

        public FreshliServiceBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public void Register()
        {
            this.RegisterBaseCommand();
            this.RegisterScanCommand();
            this.RegisterAuthCommand();
        }

        public void RegisterBaseCommand()
        {
            this.Services.AddNamedScoped<IOutputFormatter, JsonOutputFormatter>(FormatType.Json);
            this.Services.AddNamedScoped<IOutputFormatter, CsvOutputFormatter>(FormatType.Csv);
            this.Services.AddNamedScoped<IOutputFormatter, YamlOutputFormatter>(FormatType.Yaml);
            this.Services.AddNamedScoped<IOutputStrategy, FileOutputStrategy>(OutputStrategyType.File);
            this.Services.AddNamedScoped<IOutputStrategy, ConsoleOutputStrategy>(OutputStrategyType.Console);
            this.Services.AddScoped<ICommandRunnerFactory, IoCCommandRunnerFactory>(serviceProvider => new IoCCommandRunnerFactory(serviceProvider));
        }

        public void RegisterScanCommand()
        {
            this.Services.AddScoped<Runner>();
            this.Services.AddTransient<ICommandRunner<ScanCommandOptions>, ScanCommandRunner>();
            this.Services.AddOptions<ScanCommandOptions>().BindCommandLine();
        }

        public void RegisterAuthCommand()
        {
            this.Services.AddScoped<ICommandRunner<AuthCommandOptions>, AuthCommandRunner>();
            this.Services.AddOptions<AuthCommandOptions>().BindCommandLine();
        }
    }
}
