using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
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
            Services = services;
        }

        public void Register()
        {
            RegisterBaseCommand();
            RegisterScanCommand();
        }

        public void RegisterBaseCommand()
        {
            Services.AddNamedScoped<IOutputFormatter, JsonOutputFormatter>(FormatType.Json);
            Services.AddNamedScoped<IOutputFormatter, CsvOutputFormatter>(FormatType.Csv);
            Services.AddNamedScoped<IOutputFormatter, YamlOutputFormatter>(FormatType.Yaml);
            Services.AddNamedScoped<IOutputStrategy, FileOutputStrategy>(OutputStrategyType.File);
            Services.AddNamedScoped<IOutputStrategy, ConsoleOutputStrategy>(OutputStrategyType.Console);            
        }

        public void RegisterScanCommand()
        {
            Services.AddScoped<Runner>();
            Services.AddScoped<ICommandRunner<ScanCommandOptions>, ScanCommandRunner>();
            Services.AddOptions<ScanCommandOptions>().BindCommandLine();
        }
    }
}
