using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class BaseCommandServiceBuilder
    {
        public void Register(IServiceCollection collection)
        {
            collection.AddNamedScoped<IOutputFormatter, JsonOutputFormatter>(FormatType.Json);
            collection.AddNamedScoped<IOutputFormatter, CsvOutputFormatter>(FormatType.Csv);
            collection.AddNamedScoped<IOutputFormatter, YamlOutputFormatter>(FormatType.Yaml);
            collection.AddNamedScoped<IOutputStrategy, FileOutputStrategy>(OutputStrategyType.File);
            collection.AddNamedScoped<IOutputStrategy, ConsoleOutputStrategy>(OutputStrategyType.Console);
            collection.AddScoped<ICommandRunnerFactory, IoCCommandRunnerFactory>(serviceProvider => new IoCCommandRunnerFactory(serviceProvider));
        }
    }
}
