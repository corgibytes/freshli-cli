using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Test.Common;
using Freshli;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using YamlDotNet.Serialization;

namespace Corgibytes.Freshli.Cli.Test.Formatters
{
    public class OutputFormatterTest : FreshliTest
    {
        private static JsonOutputFormatter JsonFormatter { get; }
        private static YamlOutputFormatter YamlFormatter { get; }
        private static CsvOutputFormatter CsvFormatter { get; }

        private static readonly MetricsResult metricsResultTestData;

        private static readonly IList<MetricsResult> metricsResultListTestData;

        public OutputFormatterTest( ITestOutputHelper output ) : base(output) { }

        static OutputFormatterTest()
        {
            JsonFormatter = new();
            YamlFormatter = new();
            CsvFormatter = new();

            DateTime date = new(2021, 11, 21);
            string sha = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
            LibYearResult libYearResult = new();
            libYearResult.Add(new LibYearPackageResult("polyglot", "0.3.3", new DateTime(2011, 11, 01), "0.3.3", new DateTime(2011, 11, 01), 0.0, false, false));

            metricsResultTestData = new(date, sha, libYearResult);
            metricsResultListTestData = new List<MetricsResult>() { metricsResultTestData, metricsResultTestData };
        }

        [Theory]
        [MemberData(nameof(FormatterTypeCheckData))]

        public void Check_FormatterType_IsExpectedType( IOutputFormatter formatter, FormatType expectedType )
        {
            Assert.Equal(expectedType, formatter.Type);
        }

        [Theory]
        [MemberData(nameof(Formatters))]
        public void Send_NullObject_ReturnsArgumentNullException( IOutputFormatter formatter )
        {
            MetricsResult result = null;
            Assert.Throws<ArgumentNullException>(() => formatter.Format(result));
        }


        [Theory]
        [MemberData(nameof(SerializedData))]
        public void Send_Object_ReturnsSerializedEntity( string expectedSerialization, string actualSerialization, FormatType formatType )
        {
            this.Output.WriteLine($"Testing {formatType} serialization");
            Assert.Equal(expectedSerialization, actualSerialization);
        }

        public static IEnumerable<object[]> FormatterTypeCheckData =>
          new List<object[]>
          {
                new object[] { JsonFormatter, FormatType.json},
                new object[] { YamlFormatter, FormatType.yaml},
                new object[] { CsvFormatter, FormatType.csv},
          };

        public static IEnumerable<object[]> Formatters =>
           new List<object[]>
           {
                new object[] { JsonFormatter},
                new object[] { YamlFormatter},
                new object[] { CsvFormatter },
           };

        public static IEnumerable<object[]> SerializedData =>
   new List<object[]>
   {
                new object[] { JsonFormatter.Format(metricsResultTestData), JsonConvert.SerializeObject(metricsResultTestData, Formatting.Indented),JsonFormatter.Type },
                new object[] { YamlFormatter.Format(metricsResultTestData), new Serializer().Serialize(metricsResultTestData), YamlFormatter.Type  },
                new object[] { CsvFormatter.Format(metricsResultTestData), CsvSerializer.SerializeToCsv(new List<MetricsResult>() { metricsResultTestData }), CsvFormatter.Type },

                new object[] { JsonFormatter.Format(metricsResultListTestData), JsonConvert.SerializeObject(metricsResultListTestData, Formatting.Indented), JsonFormatter.Type },
                new object[] { YamlFormatter.Format(metricsResultListTestData), new Serializer().Serialize(metricsResultListTestData), YamlFormatter.Type  },
                new object[] { CsvFormatter.Format(metricsResultListTestData), CsvSerializer.SerializeToCsv(metricsResultListTestData), CsvFormatter.Type  },

                new object[] { JsonFormatter.Format<MetricsResult>(new List<MetricsResult>()), JsonConvert.SerializeObject(new List<MetricsResult>(), Formatting.Indented), JsonFormatter.Type },
                new object[] { YamlFormatter.Format<MetricsResult>(new List<MetricsResult>()), new Serializer().Serialize(new List<MetricsResult>()), YamlFormatter.Type },
                new object[] { CsvFormatter.Format<MetricsResult>(new List<MetricsResult>()), CsvSerializer.SerializeToCsv(new List<MetricsResult>()), CsvFormatter.Type },
   };
    }
}
