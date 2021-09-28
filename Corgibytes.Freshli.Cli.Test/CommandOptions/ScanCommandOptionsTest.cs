using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.CommandOptions
{
    public class ScanCommandOptionsTest : FreshliTest
    {
        private static string TempPath { get; } = Path.GetTempPath();

        public ScanCommandOptionsTest(ITestOutputHelper output) : base(output) { }

        [Theory]
        [MemberData(nameof(ScanOptionsArgs))]
        public void Send_Args_ReturnsScanOptions(string[] args, string expectedPath, FormatType expectedFormat, IList<OutputStrategyType> expectedOutput)
        {
            CommandLineBuilder cmBuilder = Program.CreateCommandLineBuilder();
            Parser parser = new(cmBuilder.Command);

            ParseResult result = parser.Parse(args);

            DirectoryInfo path = result.ValueForArgument<DirectoryInfo>("path");
            FormatType formatType = result.ValueForOption<FormatType>("--format");
            FormatType formatTypeFromAlias = result.ValueForOption<FormatType>("-f");

            IEnumerable<OutputStrategyType> outputStrategyTypes = result.ValueForOption<IEnumerable<OutputStrategyType>>("--output");
            IEnumerable<OutputStrategyType> outputStrategyTypesFromAlias = result.ValueForOption<IEnumerable<OutputStrategyType>>("-o");

            formatType.Should().Be(formatTypeFromAlias);

            outputStrategyTypes.Should().NotBeEmpty()
                .And.Equal(outputStrategyTypesFromAlias);

            path.Should().NotBeNull();
            path.FullName.Should().NotBeEmpty()
                .And.Be(expectedPath);

            formatType.Should()
                .Be(expectedFormat);

            outputStrategyTypes.Should().NotContainNulls();

            //Veryfy all required ones are present
            expectedOutput.Should().BeSubsetOf(outputStrategyTypes);

            //Veryfy there aren't extra output stragetegies added
            outputStrategyTypes.Should().BeSubsetOf(expectedOutput);
        }

        public static IEnumerable<object[]> ScanOptionsArgs =>
              new List<object[]>
              {
                    new object[] { new string[] { "scan", TempPath, "--format", "json", "--output", "console"}, TempPath, FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", TempPath, "--Format", "JSON", "--output", "CONSOLE" }, TempPath, FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", TempPath, "--format", "csv", "--output", "file", "--output", "console" }, TempPath, FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.File, OutputStrategyType.Console }},
                    new object[] { new string[] { "scan", TempPath, "--format", "yaml", "--output", "file" }, TempPath, FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.File }},
                    new object[] { new string[] { "scan", TempPath, "-f", "json", "-o", "console" }, TempPath, FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", TempPath, "-f", "csv", "-o", "file", "-o", "console" }, TempPath, FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.File, OutputStrategyType.Console }},
                    new object[] { new string[] { "scan", TempPath, "-f", "Csv", "-o", "FILE", "-o", "console" }, TempPath, FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.File, OutputStrategyType.Console }},
                    new object[] { new string[] { "scan", TempPath, "-f", "yaml", "-o", "console" }, TempPath, FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", TempPath, "--format", "yaml", "-o", "file" }, TempPath, FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.File }},
                    new object[] { new string[] { "scan", TempPath, "-f", "yaml", "--output", "console","-o", "file" }, TempPath, FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.Console, OutputStrategyType.File } },

                    //It should configure the default formatter
                    new object[] { new string[] { "scan", TempPath , "--output", "console" }, TempPath, FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", TempPath, "-o", "file" }, TempPath, FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.File} },                    

                    //It should configure the default output
                    new object[] { new string[] { "scan", TempPath, "--format", "yaml" }, TempPath, FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", TempPath, "-f", "csv" }, TempPath, FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.Console } },

                    //It should configure the default formatter and default output
                    new object[] { new string[] { "scan", TempPath }, TempPath, FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } }
              };
    }
}
