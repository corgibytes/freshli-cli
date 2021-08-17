using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.IoC;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Corgibytes.Freshli.Cli.Test.Factories
{
    public class IoCCommandRunnerFactoryTest
    {

        [Theory]
        [MemberData(nameof(ScanOptionsArgs))]
        public void Send_ScanOptions_CreateScanRunner( FormatType format, IList<OutputStrategyType> outputTypes )
        {
            IHostBuilder host = Host.CreateDefaultBuilder()
              .ConfigureServices((_, services) => {
                  FreshliServiceBuilder.Register(services);
              });

            IoCCommandRunnerFactory commandRunnerFactory = new(host.Build().Services);
            ScanCommandOptions scanOptions = new();
            scanOptions.Format = format;
            scanOptions.Output = outputTypes;

            ScanCommandRunner runner = commandRunnerFactory.CreateScanCommandRunner(scanOptions) as ScanCommandRunner;

            runner.Should().NotBeNull();
            runner.OutputFormatter.Type.Should().Be(format);            

            IEnumerable<OutputStrategyType> actualOutputTypes = runner.OutputStrategies.Select(os => os.Type);

            //Veryfy all required ones are present
            actualOutputTypes.Should().BeSubsetOf(outputTypes);

            //Veryfy there aren't extra output stragetegies added
            outputTypes.Should().BeSubsetOf(actualOutputTypes);

        }

        public static IEnumerable<object[]> ScanOptionsArgs =>
        new List<object[]>
        {
            new object[] { FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.Console } },
            new object[] { FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console, OutputStrategyType.File } },
        };
    }
}

