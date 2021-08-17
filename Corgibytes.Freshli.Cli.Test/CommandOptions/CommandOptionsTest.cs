using System.Collections.Generic;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.CommandOptions
{
    public class CommandOptionsTest : FreshliTest
    {
        public CommandOptionsTest(ITestOutputHelper output) : base(output) { }

        [Theory]
        [MemberData(nameof(CommandOptionsTypeCheckData))]

        public void Check_FormatterType_IsExpectedType(ICommandOptions options, CommandOptionType expectedType)
        {
            options.Type.Should().Be(expectedType);
        }

        public static IEnumerable<object[]> CommandOptionsTypeCheckData =>
            new List<object[]>
            {
                new object[] { new ScanCommandOptions(), CommandOptionType.Scan },
                new object[] { new AuthCommandOptions(), CommandOptionType.Auth }
            };
    }
}
