using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.IoC;
using Corgibytes.Freshli.Cli.OutputStrategies;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Factories
{
    public class IoCCommandRunnerFactoryTest
    {

        [Theory]
        [MemberData(nameof(ScanOptionsArgs))]
        public void Send_ScanOptions_CreateScanRunner(FormatType format, IList<OutputStrategyType> outputTypes)
        {
            IHostBuilder host = Host.CreateDefaultBuilder()
              .ConfigureServices((_, services) =>
              {
                  FreshliServiceBuilder.Register(services);
              });

            IoCCommandRunnerFactory commandRunnerFactory = new(host.Build().Services);
            ScanCommandOptions scanOptions = new()
            {
                Format = format,
                Output = outputTypes
            };

            ScanCommandRunner runner = commandRunnerFactory.CreateScanCommandRunner(scanOptions) as ScanCommandRunner;
            runner.OutputFormatter.Type.Should().Be(format);

            IEnumerable<OutputStrategyType> actualOutputTypes = runner.OutputStrategies.Select(os => os.Type);

            //Veryfy all required ones are present
            actualOutputTypes.Should().BeSubsetOf(outputTypes);

            //Veryfy there aren't extra output stragetegies added
            outputTypes.Should().BeSubsetOf(actualOutputTypes);

        }

        [Fact]
        public void AuthCommandRunner_Should_NotBeNull()
        {
            IHostBuilder host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    FreshliServiceBuilder.Register(services);
                });

            IoCCommandRunnerFactory commandRunnerFactory = new(host.Build().Services);
            AuthCommandRunner runner = commandRunnerFactory.CreateAuthCommandRunner() as AuthCommandRunner;
            runner.Should().NotBeNull();
        }

        [Fact]
        public void ScanCommandRunner_Invoke_Throws_Exception_OnNullOptions()
        {
            IHostBuilder host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    FreshliServiceBuilder.Register(services);
                });

            IoCCommandRunnerFactory commandRunnerFactory = new(host.Build().Services);
            commandRunnerFactory
                .Invoking(x => x.CreateScanCommandRunner(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ScanCommandRunner_Should_NotBeNull()
        {
            IHostBuilder host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    FreshliServiceBuilder.Register(services);
                });

            IoCCommandRunnerFactory commandRunnerFactory = new(host.Build().Services);
            ICommandRunner<ScanCommandOptions> runner = commandRunnerFactory.CreateScanCommandRunner(new ScanCommandOptions());
            runner.Should().NotBeNull();
        }

        public static IEnumerable<object[]> ScanOptionsArgs =>
        new List<object[]>
        {
            new object[] { FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.Console } },
            new object[] { FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console, OutputStrategyType.File } },
        };
    }
}

