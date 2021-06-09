using CommandLine;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Test.Common;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Options
{
    public class ScanOptionsTest : FreshliTest
    {
        public ScanOptionsTest( ITestOutputHelper output ) : base(output) { }

        [Theory]
        [MemberData(nameof(ScanOptionsArgs))]
        public void Send_Args_ReturnsScanOptions( string[] args, string expectedPath, FormatType expectedFormat, IList<OutputStrategyType> expectedOutput )
        {
            this.Output.WriteLine(string.Join(',', args));
            Parser.Default.ParseArguments<ScanOptions, AuthOptions>(args).MapResult(
               ( ScanOptions opts ) => AssertScanOptions(opts, expectedPath, expectedFormat, expectedOutput),
               ( IEnumerable<Error> errs ) => AssertError(errs, args));
        }

        private int AssertError( IEnumerable<Error> errors, string[] args )
        {
            this.Output.WriteLine($"Send_Args_ReturnsScanOptions - The following args can't be parsed - {string.Join(',', args)}");
            Assert.True(false, $"There was an error trying to parce the following args - {string.Join(',', args)}"); ;
            return 1;
        }

        private int AssertScanOptions( ScanOptions options, string expectedPath, FormatType expectedFormat, IList<OutputStrategyType> expectedOutput )
        {
            Assert.Equal(expectedPath, options.Path);
            Assert.Equal(expectedFormat, options.Format);

            //Veryfy all required ones are present
            foreach(OutputStrategyType type in expectedOutput)
                Assert.Contains(type, options.Output);

            //Veryfy there aren't extra output stragetegies added
            foreach(OutputStrategyType type in options.Output)
                Assert.Contains(type, expectedOutput);

            return 0;
        }


        public static IEnumerable<object[]> ScanOptionsArgs =>
              new List<object[]>
              {
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "json", "--output", "console" }, "c:\\tmp", FormatType.json, new List<OutputStrategyType>() { OutputStrategyType.console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "csv", "--output", "file,console" }, "c:\\tmp", FormatType.csv, new List<OutputStrategyType>() { OutputStrategyType.file, OutputStrategyType.console }},
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "yaml", "--output", "file" }, "c:\\tmp", FormatType.yaml, new List<OutputStrategyType>() { OutputStrategyType.file }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "json", "-o", "console" }, "c:\\tmp", FormatType.json, new List<OutputStrategyType>() { OutputStrategyType.console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "csv", "-o", "file,console" }, "c:\\tmp", FormatType.csv, new List<OutputStrategyType>() { OutputStrategyType.file, OutputStrategyType.console }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "yaml", "-o", "console" }, "c:\\tmp", FormatType.yaml, new List<OutputStrategyType>() { OutputStrategyType.console }},
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "yaml", "-o", "file" }, "c:\\tmp", FormatType.yaml, new List<OutputStrategyType>() { OutputStrategyType.file }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "yaml", "--output", "console,file" }, "c:\\tmp", FormatType.yaml, new List<OutputStrategyType>() { OutputStrategyType.console, OutputStrategyType.file } },
                    //It should send the default formatter
                    new object[] { new string[] { "scan", "c:\\tmp", "--output", "console" }, "c:\\tmp", FormatType.json, new List<OutputStrategyType>() { OutputStrategyType.console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "-o", "file" }, "c:\\tmp", FormatType.json, new List<OutputStrategyType>() { OutputStrategyType.file} },                    
                    //It should send the default output
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "yaml" }, "c:\\tmp", FormatType.yaml, new List<OutputStrategyType>() { OutputStrategyType.console}},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "csv" }, "c:\\tmp", FormatType.csv, new List<OutputStrategyType>() { OutputStrategyType.console}},
                    //It should send the default formatter and default output
                    new object[] { new string[] { "scan", "c:\\tmp"}, "c:\\tmp", FormatType.json, new List<OutputStrategyType>() { OutputStrategyType.console}}
              };
    }
}
