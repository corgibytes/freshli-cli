using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Lib;
using FluentAssertions;
using Newtonsoft.Json;
using ServiceStack.Text;
using Xunit;
using Xunit.Abstractions;
using YamlDotNet.Serialization;

namespace Corgibytes.Freshli.Cli.Test.Formatters;

[IntegrationTest]
public class OutputFormatterTest : FreshliTest
{
    private static readonly MetricsResult s_metricsResultTestData;

    private static readonly IList<MetricsResult> s_metricsResultListTestData;

    static OutputFormatterTest()
    {
        JsonFormatter = new JsonOutputFormatter();
        YamlFormatter = new YamlOutputFormatter();
        CsvFormatter = new CsvOutputFormatter();

        var date = new DateTime(2021, 11, 21);
        var sha = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
        var libYearResult = new LibYearResult
        {
            new("polyglot", "0.3.3", new DateTime(2011, 11, 01), "0.3.3", new DateTime(2011, 11, 01), 0.0, false,
                false)
        };

        s_metricsResultTestData = new MetricsResult(date, sha, libYearResult);
        s_metricsResultListTestData = new List<MetricsResult>
        {
            s_metricsResultTestData,
            s_metricsResultTestData
        };
    }

    public OutputFormatterTest(ITestOutputHelper output) : base(output)
    {
    }

    private static JsonOutputFormatter JsonFormatter { get; }
    private static YamlOutputFormatter YamlFormatter { get; }
    private static CsvOutputFormatter CsvFormatter { get; }

    public static IEnumerable<object[]> FormatterTypeCheckData =>
        new List<object[]>
        {
            new object[] { JsonFormatter, FormatType.Json },
            new object[] { YamlFormatter, FormatType.Yaml },
            new object[] { CsvFormatter, FormatType.Csv }
        };

    public static IEnumerable<object[]> Formatters =>
        new List<object[]>
        {
            new object[] { JsonFormatter },
            new object[] { YamlFormatter },
            new object[] { CsvFormatter }
        };

    public static IEnumerable<object[]> SerializedData =>
        new List<object[]>
        {
            new object[]
            {
                JsonFormatter.Format(s_metricsResultTestData),
                JsonConvert.SerializeObject(s_metricsResultTestData, Formatting.Indented), JsonFormatter.Type
            },
            new object[]
            {
                YamlFormatter.Format(s_metricsResultTestData),
                new Serializer().Serialize(s_metricsResultTestData), YamlFormatter.Type
            },
            new object[]
            {
                CsvFormatter.Format(s_metricsResultTestData),
                CsvSerializer.SerializeToCsv(new List<MetricsResult> { s_metricsResultTestData }),
                CsvFormatter.Type
            },
            new object[]
            {
                JsonFormatter.Format(s_metricsResultListTestData),
                JsonConvert.SerializeObject(s_metricsResultListTestData, Formatting.Indented),
                JsonFormatter.Type
            },
            new object[]
            {
                YamlFormatter.Format(s_metricsResultListTestData),
                new Serializer().Serialize(s_metricsResultListTestData), YamlFormatter.Type
            },
            new object[]
            {
                CsvFormatter.Format(s_metricsResultListTestData),
                CsvSerializer.SerializeToCsv(s_metricsResultListTestData), CsvFormatter.Type
            },
            new object[]
            {
                JsonFormatter.Format<MetricsResult>(new List<MetricsResult>()),
                JsonConvert.SerializeObject(new List<MetricsResult>(), Formatting.Indented), JsonFormatter.Type
            },
            new object[]
            {
                YamlFormatter.Format<MetricsResult>(new List<MetricsResult>()),
                new Serializer().Serialize(new List<MetricsResult>()), YamlFormatter.Type
            },
            new object[]
            {
                CsvFormatter.Format<MetricsResult>(new List<MetricsResult>()),
                CsvSerializer.SerializeToCsv(new List<MetricsResult>()), CsvFormatter.Type
            }
        };

    [Theory]
    [MemberData(nameof(FormatterTypeCheckData))]
    public void Check_FormatterType_IsExpectedType(IOutputFormatter formatter, FormatType expectedType) =>
        formatter.Type.Should().Be(expectedType);

    [Theory]
    [MemberData(nameof(Formatters))]
    public void Send_NullObject_ReturnsArgumentNullException(IOutputFormatter formatter)
    {
        MetricsResult? result = null;
        formatter.Invoking(x => x.Format(result))
            .Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(Formatters))]
    public void Send_NullList_ReturnsArgumentNullException(IOutputFormatter formatter)
    {
        IList<MetricsResult>? results = null;
        formatter.Invoking(x => x.Format(results!))
            .Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(SerializedData))]
    public void Send_Object_ReturnsSerializedEntity(string expectedSerialization, string actualSerialization,
        FormatType formatType)
    {
        Output.WriteLine($"Testing {formatType} serialization");
        Assert.Equal(expectedSerialization, actualSerialization);
    }
}
