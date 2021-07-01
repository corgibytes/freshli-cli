using Autofac;
using Autofac.Extras.DynamicProxy;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class BaseCommandModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<JsonOutputFormatter>()
                .As<IOutputFormatter>()
                .Keyed<IOutputFormatter>(FormatType.json)
                .EnableClassInterceptors();
           
            builder.RegisterType<CsvOutputFormatter>()
                .As<IOutputFormatter>()
                .Keyed<IOutputFormatter>(FormatType.csv)
                .EnableClassInterceptors();
            
            builder.RegisterType<YamlOutputFormatter>()
                .As<IOutputFormatter>()
                .Keyed<IOutputFormatter>(FormatType.yaml)
                .EnableClassInterceptors();

            builder.RegisterType<FileOutputStrategy>()
                .As<IOutputStrategy>()
                .Keyed<IOutputStrategy>(OutputStrategyType.file)
                .EnableClassInterceptors();
            
            builder.RegisterType<ConsoleOutputStrategy>()
                .As<IOutputStrategy>()
                .Keyed<IOutputStrategy>(OutputStrategyType.console)
                .EnableClassInterceptors();
        }
    }
}
