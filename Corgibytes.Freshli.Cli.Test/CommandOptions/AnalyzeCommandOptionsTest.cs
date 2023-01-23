using System.Collections.Generic;
using System.CommandLine.Parsing;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.CommandOptions;

[IntegrationTest]
public class AnalyzeCommandOptionsTest : FreshliTest
{
    private const string DefaultGitPath = "git";
    private const string DefaultHistoryInterval = "1m";
    private const bool DefaultCommitHistory = false;
    private const bool DefaultLatestOnly = false;

    public AnalyzeCommandOptionsTest(ITestOutputHelper output) : base(output)
    {
    }

    public static IEnumerable<object?[]> AnalyzeOptionsArgs =>
        new List<object?[]>
        {
            // If passing no arguments, the default git path should be 'git'
            new object?[]
            {
                new[] { "analyze" }, "git", null, DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly
            },
            // Specific git path expected
            new object?[]
            {
                new[] { "analyze", "--git-path", "/usr/bin/local/git" }, "/usr/bin/local/git", null,
                DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly
            },
            // Specific branch expected
            new object?[]
            {
                new[] { "analyze", "--branch", "feature-fix-final.2.0" }, DefaultGitPath, "feature-fix-final.2.0",
                DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly
            },
            // Entire commit history expected
            new object?[]
            {
                new[] { "analyze", "--commit-history" }, DefaultGitPath, null, true, DefaultHistoryInterval,
                DefaultLatestOnly
            },
            // Three yearly history interval expected
            new object?[]
            {
                new[] { "analyze", "--history-interval", "3y" }, DefaultGitPath, null, DefaultCommitHistory, "3y",
                DefaultLatestOnly
            },
            // Latest only
            new object?[]
            {
                new[] { "analyze", "--latest-only" }, DefaultGitPath, null, DefaultCommitHistory,
                DefaultHistoryInterval, true
            }
        };

    [Theory]
    [MemberData(nameof(AnalyzeOptionsArgs))]
    public void Send_Args_ReturnsAnalyzeOptions(
        string[] args, string expectedGitPath, string? expectedBranch, bool expectedCommitHistory,
        string expectedHistoryInterval, bool expectedLatestOnly
    )
    {
        var builder = new Program().CreateCommandLineBuilder();
        var parser = new Parser(builder.Command);

        var result = parser.Parse(args);

        var gitPath = result.GetOptionValueByName<string>("git-path");
        gitPath.Should().NotBeEmpty().And.Be(expectedGitPath);

        if (expectedBranch != null)
        {
            var branch = result.GetOptionValueByName<string>("branch");
            branch.Should().NotBeEmpty().And.Be(expectedBranch);
        }

        var commitHistory = result.GetOptionValueByName<bool>("commit-history");
        commitHistory.Should().Be(expectedCommitHistory);

        var historyInterval = result.GetOptionValueByName<string>("history-interval");
        historyInterval.Should().NotBeEmpty().And.Be(expectedHistoryInterval);

        var latestOnly = result.GetOptionValueByName<bool>("latest-only");
        latestOnly.Should().Be(expectedLatestOnly);
    }

    [Fact]
    public void Send_Args_ReturnsAnalyzeArguments()
    {
        var builder = new Program().CreateCommandLineBuilder();
        var parser = new Parser(builder.Command);

        var result = parser.Parse(new[] { "analyze", "https://github.com/corgibytes/freshli-fixture-ruby-nokotest" });

        var repositoryLocation = result.GetArgumentValueByName<string>("repository-location");
        repositoryLocation.Should().NotBeEmpty().And.Be("https://github.com/corgibytes/freshli-fixture-ruby-nokotest");
    }
}
