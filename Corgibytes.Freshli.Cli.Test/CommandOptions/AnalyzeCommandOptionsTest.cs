using System.Collections.Generic;
using System.CommandLine.Parsing;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.CommandOptions;

[IntegrationTest]
public class AnalyzeCommandOptionsTest : FreshliTest
{
    private const string ExpectedGitPath = "git";
    private const string ExpectedBranch = "";
    private const int ExpectedWorkerCount = 0;
    private const string ExpectedHistoryInterval = "1m";
    private const bool ExpectedCommitHistory = false;

    public AnalyzeCommandOptionsTest(ITestOutputHelper output) : base(output)
    {
    }

    public static IEnumerable<object[]> AnalyzeOptionsArgs =>
        new List<object[]>
        {
            // If passing no arguments, the default git path should be 'git'
            new object[]
            {
                new[] { "analyze" }, "git", ExpectedBranch, ExpectedCommitHistory, ExpectedHistoryInterval, ExpectedWorkerCount
            },
            // Specific git path expected
            new object[]
            {
                new[] { "analyze" , "--git-path", "/usr/bin/local/git" }, "/usr/bin/local/git", ExpectedBranch, ExpectedCommitHistory, ExpectedHistoryInterval,ExpectedWorkerCount
            },
            // Specific branch expected
            new object[]
            {
                new[] { "analyze" , "--branch", "feature-fix-final.2.0" }, ExpectedGitPath, "feature-fix-final.2.0", ExpectedCommitHistory, ExpectedHistoryInterval,ExpectedWorkerCount
            },
            // Entire commit history expected
            new object[]
            {
                new[] { "analyze" , "--commit-history" }, ExpectedGitPath, ExpectedBranch, true, ExpectedHistoryInterval, ExpectedWorkerCount
            },
            // Three yearly history interval expected
            new object[]
            {
                new[] { "analyze" , "--history-interval", "3y" }, ExpectedGitPath, ExpectedBranch, ExpectedCommitHistory, "3y", ExpectedWorkerCount
            },
            // 24 workers expected
            new object[]
            {
                new[] { "analyze" , "--workers", "24" }, ExpectedGitPath, ExpectedBranch, ExpectedCommitHistory, ExpectedHistoryInterval, 24
            }
        };

    [Theory]
    [MemberData(nameof(AnalyzeOptionsArgs))]
    public void Send_Args_ReturnsAnalyzeOptions(
        string[] args, string expectedGitPath, string expectedBranch, bool expectedCommitHistory, string expectedHistoryInterval, int expectedWorkers
    )
    {
        var cmBuilder = Program.CreateCommandLineBuilder();
        var parser = new Parser(cmBuilder.Command);

        var result = parser.Parse(args);

        var gitPath = result.GetOptionValueByName<string>("git-path");
        gitPath?.Should().NotBeEmpty().And.Be(expectedGitPath);

        var branch = result.GetOptionValueByName<string>("branch");
        branch?.Should().NotBeEmpty().And.Be(expectedBranch);

        var commitHistory = result.GetOptionValueByName<bool>("commit-history");
        commitHistory.Should().Be(expectedCommitHistory);

        var historyInterval = result.GetOptionValueByName<string>("history-interval");
        historyInterval?.Should().NotBeEmpty().And.Be(expectedHistoryInterval);

        var workers = result.GetOptionValueByName<int>("workers");
        workers.Should().Be(expectedWorkers);
    }

    [Fact]
    public void Send_Args_ReturnsAnalyzeArguments()
    {
        var cmBuilder = Program.CreateCommandLineBuilder();
        var parser = new Parser(cmBuilder.Command);

        var result = parser.Parse(new[] { "analyze", "https://github.com/corgibytes/freshli-fixture-ruby-nokotest" });

        var repositoryLocation = result.GetArgumentValueByName<string>("repository-location");
        repositoryLocation?.Should().NotBeEmpty().And.Be("https://github.com/corgibytes/freshli-fixture-ruby-nokotest");
    }
}

