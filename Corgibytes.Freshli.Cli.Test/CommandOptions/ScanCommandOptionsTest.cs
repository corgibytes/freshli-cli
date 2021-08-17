using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
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

            outputStrategyTypes.Should().NotContainNulls()
            .And.NotContain(OutputStrategyType.None);

            //Veryfy all required ones are present
            expectedOutput.Should().BeSubsetOf(outputStrategyTypes);

            //Veryfy there aren't extra output stragetegies added
            outputStrategyTypes.Should().BeSubsetOf(expectedOutput);
        }

        [Fact]
        public void Verify_ScanCommandOptionsType()
        {
            ScanCommandOptions options = new();
            options.Type.Should().Be(CommandOptionType.Scan);
        }

        public static IEnumerable<object[]> ScanOptionsArgs =>
              new List<object[]>
              {
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "json", "--output", "console"}, "c:\\tmp", FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "--Format", "JSON", "--output", "CONSOLE" }, "c:\\tmp", FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "csv", "--output", "file", "--output", "console" }, "c:\\tmp", FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.File, OutputStrategyType.Console }},
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "yaml", "--output", "file" }, "c:\\tmp", FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.File }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "json", "-o", "console" }, "c:\\tmp", FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "csv", "-o", "file", "-o", "console" }, "c:\\tmp", FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.File, OutputStrategyType.Console }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "Csv", "-o", "FILE", "-o", "console" }, "c:\\tmp", FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.File, OutputStrategyType.Console }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "yaml", "-o", "console" }, "c:\\tmp", FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "yaml", "-o", "file" }, "c:\\tmp", FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.File }},
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "yaml", "--output", "console","-o", "file" }, "c:\\tmp", FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.Console, OutputStrategyType.File } },
                    //It should send the default formatter
                    new object[] { new string[] { "scan", "c:\\tmp", "--output", "console" }, "c:\\tmp", FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "-o", "file" }, "c:\\tmp", FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.File} },                    
                    //It should send the default output
                    new object[] { new string[] { "scan", "c:\\tmp", "--format", "yaml" }, "c:\\tmp", FormatType.Yaml, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    new object[] { new string[] { "scan", "c:\\tmp", "-f", "csv" }, "c:\\tmp", FormatType.Csv, new List<OutputStrategyType>() { OutputStrategyType.Console } },
                    //It should send the default formatter and default output
                    new object[] { new string[] { "scan", "c:\\tmp"}, "c:\\tmp", FormatType.Json, new List<OutputStrategyType>() { OutputStrategyType.Console } }
              };
    }
}

