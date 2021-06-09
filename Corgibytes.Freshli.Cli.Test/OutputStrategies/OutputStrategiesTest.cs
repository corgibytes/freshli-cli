using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Test.Common;
using Freshli;
using Moq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.OutputStrategies
{
    public class OutputStrategiesTest : FreshliTest
    {
        public static ConsoleOutputStrategy ConsoleOutputStrategy { get; }
        public static FileOutputStrategy FileOutputStrategy { get; }

        public OutputStrategiesTest( ITestOutputHelper output ) : base(output) { }

        static OutputStrategiesTest()
        {
            ConsoleOutputStrategy = new ();
            FileOutputStrategy = new();
        }

        [Theory]
        [MemberData(nameof(OutputStrategies))]
        public void Send_MetricsResult_CallFormatterOnce( IOutputStrategy strategy )
        {
            var formatterMock = new Mock<IOutputFormatter>();
            var result = new List<MetricsResult>();
            var optionsMock = new Mock<ScanOptions>();

            formatterMock.Setup(f => f.Format<MetricsResult>(result)).Returns("formatted text");
            strategy.Send(result, formatterMock.Object, optionsMock.Object);
            formatterMock.Verify(f => f.Format<MetricsResult>(result), Times.Once());
        }

        [Theory]
        [MemberData(nameof(OutputStrategiesTypeCheckData))]

        public void Check_FormatterType_IsExpectedType( IOutputStrategy outputStrategy, OutputStrategyType expectedType )
        {
            Assert.Equal(expectedType, outputStrategy.Type);
        }


        public static IEnumerable<object[]> OutputStrategies =>
         new List<object[]>
         {
                new object[] { ConsoleOutputStrategy},
                new object[] { FileOutputStrategy},
         };

        public static IEnumerable<object[]> OutputStrategiesTypeCheckData =>
          new List<object[]>
          {
                new object[] { ConsoleOutputStrategy, OutputStrategyType.console},
                new object[] { FileOutputStrategy, OutputStrategyType.file},
          };
    }
}
