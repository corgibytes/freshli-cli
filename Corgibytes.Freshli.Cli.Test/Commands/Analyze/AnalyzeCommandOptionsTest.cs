using System.Collections.Generic;
using System.CommandLine.Parsing;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Commands.Analyze;

[IntegrationTest]
public class AnalyzeCommandOptionsTest : FreshliTest
{
    private const string DefaultGitPath = "git";
    private const string? DefaultBranch = null;
    private const string DefaultHistoryInterval = "1m";
    private const bool DefaultCommitHistory = false;
    private const bool DefaultLatestOnly = false;
    private const string? DefaultProjectSlug = null;

    public AnalyzeCommandOptionsTest(ITestOutputHelper output) : base(output)
    {
    }

    public static IEnumerable<object?[]> AnalyzeOptionsArgs =>
        new List<object?[]>
        {
            // If passing no arguments, the default git path should be 'git'
            new object?[]
            {
                new[] { "analyze" }, "git",
                DefaultBranch, DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly, DefaultProjectSlug
            },
            // Specific git path expected
            new object?[]
            {
                new[] { "analyze", "--git-path", "/usr/bin/local/git" },
                "/usr/bin/local/git", DefaultBranch, DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly, DefaultProjectSlug
            },
            // Specific branch expected
            new object?[]
            {
                new[] { "analyze", "--branch", "feature-fix-final.2.0" },
                DefaultGitPath, "feature-fix-final.2.0", DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly, DefaultProjectSlug
            },
            // Entire commit history expected
            new object?[]
            {
                new[] { "analyze", "--commit-history" },
                DefaultGitPath, DefaultBranch, true, DefaultHistoryInterval, DefaultLatestOnly, DefaultProjectSlug
            },
            // Three yearly history interval expected
            new object?[]
            {
                new[] { "analyze", "--history-interval", "3y" },
                DefaultGitPath, DefaultBranch, DefaultCommitHistory, "3y", DefaultLatestOnly, DefaultProjectSlug
            },
            // Latest only
            new object?[]
            {
                new[] { "analyze", "--latest-only" },
                DefaultGitPath, DefaultBranch, DefaultCommitHistory, DefaultHistoryInterval, true, DefaultProjectSlug
            },
            // Specific project slug
            new object?[]
            {
                new[] { "analyze", "--project", "org-nickname/project-nickname" },
                DefaultGitPath, DefaultBranch, DefaultCommitHistory, DefaultHistoryInterval, DefaultLatestOnly, "org-nickname/project-nickname"
            }
        };

    [Theory]
    [MemberData(nameof(AnalyzeOptionsArgs))]
    public void Send_Args_ReturnsAnalyzeOptions(
        string[] args, string expectedGitPath, string? expectedBranch, bool expectedCommitHistory,
        string expectedHistoryInterval, bool expectedLatestOnly, string? expectedProjectSlug
    )
    {
        var builder = new Program().CreateCommandLineBuilder();
        var parser = new Parser(builder.Command);

        var result = parser.Parse(args);

        var gitPath = result.GetOptionValueByName<string>("git-path");
        gitPath.Should().NotBeEmpty().And.Be(expectedGitPath);

        var branch = result.GetOptionValueByName<string>("branch");
        if (expectedBranch != null)
        {
            branch.Should().NotBeEmpty().And.Be(expectedBranch);
        }
        else
        {
            branch.Should().BeNull();
        }

        var commitHistory = result.GetOptionValueByName<bool>("commit-history");
        commitHistory.Should().Be(expectedCommitHistory);

        var historyInterval = result.GetOptionValueByName<string>("history-interval");
        historyInterval.Should().NotBeEmpty().And.Be(expectedHistoryInterval);

        var latestOnly = result.GetOptionValueByName<bool>("latest-only");
        latestOnly.Should().Be(expectedLatestOnly);

        var projectSlug = result.GetOptionValueByName<string>("project");
        if (expectedProjectSlug != null)
        {
            projectSlug.Should().NotBeEmpty().And.Be(expectedProjectSlug);
        }
        else
        {
            projectSlug.Should().BeNull();
        }
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
