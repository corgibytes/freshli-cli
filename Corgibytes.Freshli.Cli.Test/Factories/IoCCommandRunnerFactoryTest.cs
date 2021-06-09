using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Runners;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Factories
{
    public class IoCCommandRunnerFactoryTest
    {

        [Theory]
        [MemberData(nameof(ScanOptionsArgs))]
        public void Send_ScanOptions_CreateScanRunner( FormatType format, IList<OutputStrategyType> outputTypes )
        {
            IoCCommandRunnerFactory commandRunnerFactory = new();
            ScanOptions scanOptions = new();
            scanOptions.Format = format;
            scanOptions.Output = outputTypes;

            ScanCommandRunner runner = commandRunnerFactory.CreateScanRunner(scanOptions) as ScanCommandRunner;

            Assert.NotNull(runner);
            Assert.Equal(format, runner.OutputFormatter.Type);

            IEnumerable<OutputStrategyType> actualOutputTypes = runner.OutputStrategies.Select(os => os.Type);

            //Veryfy all required ones are present
            foreach(OutputStrategyType expectedType in outputTypes)
                Assert.Contains(expectedType, actualOutputTypes);

            //Veryfy there aren't extra output stragetegies added
            foreach(OutputStrategyType actualType in actualOutputTypes)
                Assert.Contains(actualType, outputTypes);

        }

        public static IEnumerable<object[]> ScanOptionsArgs =>
        new List<object[]>
        {
            new object[] { FormatType.csv, new List<OutputStrategyType>() { OutputStrategyType.console } },
            new object[] { FormatType.json, new List<OutputStrategyType>() { OutputStrategyType.console, OutputStrategyType.file } },
        };
    }
}

