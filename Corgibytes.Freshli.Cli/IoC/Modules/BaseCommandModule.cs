using Autofac;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class BaseCommandModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<JsonOutputFormatter>().As<IOutputFormatter>().Keyed<IOutputFormatter>(FormatType.json);
            builder.RegisterType<CsvOutputFormatter>().As<IOutputFormatter>().Keyed<IOutputFormatter>(FormatType.csv);
            builder.RegisterType<YamlOutputFormatter>().As<IOutputFormatter>().Keyed<IOutputFormatter>(FormatType.yaml);

            builder.RegisterType<FileOutputStrategy>().As<IOutputStrategy>().Keyed<IOutputStrategy>(OutputStrategyType.file);
            builder.RegisterType<ConsoleOutputStrategy>().As<IOutputStrategy>().Keyed<IOutputStrategy>(OutputStrategyType.console);
        }
    }
}
