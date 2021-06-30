using Corgibytes.Freshli.Lib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test
{
    public class OutputFormatterTest
    {
        private static IList<(DateTime Date, double Value, bool UpgradeAvailable, bool Skipped)> DatesAndValues
        {
            get
            {
                return new List<(
        DateTime Date, double Value, bool UpgradeAvailable, bool Skipped)>
      {
        (new DateTime(2010, 01, 01), 1.1010, false, false),
        (new DateTime(2010, 02, 01), 2.2020, false, false),
        (new DateTime(2010, 03, 01), 3.3030, false, false),
        (new DateTime(2010, 04, 01), 4.4040, false, false),
        (new DateTime(2010, 05, 01), 5.5050, false, false),
        (new DateTime(2010, 06, 01), 6.6060, false, false),
        (new DateTime(2010, 07, 01), 7.7070, false, false),
        (new DateTime(2010, 08, 01), 8.8080, false, false),
        (new DateTime(2010, 09, 01), 9.9090, false, false),
        (new DateTime(2010, 10, 01), 10.0101, false, false),
        (new DateTime(2010, 11, 01), 11.1111, true, false),
        (new DateTime(2010, 12, 01), 12.2121, true, false),
        (new DateTime(2011, 01, 01), 13.3333, false, true)
      };
            }
        }

        private static IList<MetricsResult> CreateResults()
        {
            IList<MetricsResult> results = new List<MetricsResult>();
            foreach (var dateAndValue in DatesAndValues)
            {
                var result = new LibYearResult();
                result.Add(new LibYearPackageResult
                    (
                        "test_package",
                        "1.0",
                        dateAndValue.Date,
                        "2.0",
                        DateTime.Today,
                        dateAndValue.Value,
                        dateAndValue.UpgradeAvailable,
                        dateAndValue.Skipped
                    )
                );
                results.Add(new MetricsResult(dateAndValue.Date, "N-A", result));
            }

            return results;
        }

        private static string EnglishHeader = "Date\tLibYear\tUpgradesAvailable\tSkipped";
        private static string SpanishHeader = "Fecha\tAñoLib\tActualizaciónesDisponibles\tOmitida";

        private static string ExpectedDatesAndValues(string header)
        {
            StringWriter expected = new StringWriter();
                expected.WriteLine(header);
                expected.WriteLine("2010-01-01\t1.1010\t0\t0");
                expected.WriteLine("2010-02-01\t2.2020\t0\t0");
                expected.WriteLine("2010-03-01\t3.3030\t0\t0");
                expected.WriteLine("2010-04-01\t4.4040\t0\t0");
                expected.WriteLine("2010-05-01\t5.5050\t0\t0");
                expected.WriteLine("2010-06-01\t6.6060\t0\t0");
                expected.WriteLine("2010-07-01\t7.7070\t0\t0");
                expected.WriteLine("2010-08-01\t8.8080\t0\t0");
                expected.WriteLine("2010-09-01\t9.9090\t0\t0");
                expected.WriteLine("2010-10-01\t10.0101\t0\t0");
                expected.WriteLine("2010-11-01\t11.1111\t1\t0");
                expected.WriteLine("2010-12-01\t12.2121\t1\t0");
                expected.WriteLine("2011-01-01\t0.0000\t0\t1");
                return expected.ToString();
        }

        private static void TestOutputFormatter(CultureInfo testedCulture, string expectedHeader)
        {
            CultureInfo.CurrentUICulture = testedCulture;
            CultureInfo.CurrentCulture = testedCulture;

            var results = CreateResults();

            var actual = new StringWriter();
            var formatter = new OutputFormatter(actual);
            formatter.Write(results);

            Assert.Equal(ExpectedDatesAndValues(expectedHeader), actual.ToString());
        }

        [Fact]
        public void EnglishUSLanguage()
        {
            TestOutputFormatter(CultureInfo.GetCultureInfo("en-US"), EnglishHeader);
        }

        [Fact]
        public void EnglishCanadaLanguage()
        {
            TestOutputFormatter(CultureInfo.GetCultureInfo("en-CA"), EnglishHeader);
        }


        [Fact]
        public void InvariantLanguage()
        {
            TestOutputFormatter(CultureInfo.InvariantCulture, EnglishHeader);
        }

        [Fact]
        public void SpanishLanguage()
        {
            TestOutputFormatter(CultureInfo.GetCultureInfo("es-AR"), SpanishHeader);
        }

        [Fact]
        public void UnsupportedLanguage()
        {
            TestOutputFormatter(CultureInfo.GetCultureInfo("de-DE"), EnglishHeader);
        }
    }


}
