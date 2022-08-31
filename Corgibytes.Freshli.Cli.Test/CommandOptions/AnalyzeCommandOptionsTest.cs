using System.Collections.Generic;
using System.IO;
using System.CommandLine.Parsing;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.CommandOptions;

[IntegrationTest]
public class AnalyzeCommandOptionsTest : FreshliTest
{
    public AnalyzeCommandOptionsTest(ITestOutputHelper output) : base(output)
    {
    }

    public static IEnumerable<object[]> AnalyzeOptionsArgs =>
        new List<object[]>
        {
            new object[]
            {
                new[] { "analyze" , "--git-path", "/usr/bin/local/git" }, "/usr/bin/local/git"
            },
        };

    [Theory]
    [MemberData(nameof(AnalyzeOptionsArgs))]
    public void Send_Args_ReturnsAnalyzeOptions(string[] args, string expectedGitPath)
    {
        var cmBuilder = Program.CreateCommandLineBuilder();
        var parser = new Parser(cmBuilder.Command);

        var result = parser.Parse(args);

        var gitPath = result.GetOptionValueByName<FileInfo>("git-path");

        gitPath?.FullName.Should().NotBeEmpty().And.Be(expectedGitPath);
    }
}

