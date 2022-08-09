using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Lib;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.OutputStrategies;

[UnitTest]
public class OutputStrategiesTest : FreshliTest
{
    static OutputStrategiesTest()
    {
        ConsoleOutputStrategy = new ConsoleOutputStrategy();
        FileOutputStrategy = new FileOutputStrategy();
    }

    public OutputStrategiesTest(ITestOutputHelper output) : base(output)
    {
    }

    private static ConsoleOutputStrategy ConsoleOutputStrategy { get; }
    private static FileOutputStrategy FileOutputStrategy { get; }


    public static IEnumerable<object[]> OutputStrategies =>
        new List<object[]>
        {
            new object[] { ConsoleOutputStrategy },
            new object[] { FileOutputStrategy }
        };

    public static IEnumerable<object[]> OutputStrategiesTypeCheckData =>
        new List<object[]>
        {
            new object[] { ConsoleOutputStrategy, OutputStrategyType.Console },
            new object[] { FileOutputStrategy, OutputStrategyType.File }
        };

    [Theory]
    [MemberData(nameof(OutputStrategies))]
    public void Send_MetricsResult_CallFormatterOnce(IOutputStrategy strategy)
    {
        var formatterMock = new Mock<IOutputFormatter>();
        var result = new List<ScanResult>();
        // We cannot mock DirectoryInfo, so we need to provide a real path (pwd for running test will do)
        var options = new ScanCommandOptions { Path = new DirectoryInfo(".") };

        formatterMock.Setup(f => f.Format<ScanResult>(result)).Returns("formatted text");
        strategy.Send(result, formatterMock.Object, options);
        formatterMock.Verify(f => f.Format<ScanResult>(result), Times.Once());
    }

    [Theory]
    [MemberData(nameof(OutputStrategiesTypeCheckData))]
    public void Check_FormatterType_IsExpectedType(IOutputStrategy outputStrategy, OutputStrategyType expectedType) =>
        outputStrategy.Type.Should().Be(expectedType);
}
