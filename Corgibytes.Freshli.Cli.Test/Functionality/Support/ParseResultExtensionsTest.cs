using System;
using System.CommandLine;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Support.Environment;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Support;

[IntegrationTest]
public class ParseResultExtensionsTest
{
    private readonly MainCommand _command = new(new Configuration(new Environment()));

    [Fact]
    public void GetOptionValueByNameForCommandOption()
    {
        var parseResult = _command.Parse("--loglevel debug analyze --git-path git");

        Assert.Equal("git", parseResult.GetOptionValueByName<string>("git-path"));
    }

    [Fact]
    public void GetOptionValueByNameForGlobalCommandOption()
    {
        var parseResult = _command.Parse("--loglevel debug analyze --git-path git");

        Assert.Equal("debug", parseResult.GetOptionValueByName<string>("loglevel"));
    }

    [Fact]
    public void GetOptionValueByNameWhenValueIsMissing()
    {
        var parseResult = _command.Parse("--loglevel debug analyze --git-path git");

        var exception = Assert.Throws<ArgumentException>(() => parseResult.GetOptionValueByName<string>("missing"));
        Assert.Equal(
            "No option was found with the name `missing`. Valid option names are `branch`, `cache-dir`, `commit-history`, `git-path`, `history-interval`, `latest-only`, `logfile`, `loglevel`, `project`, `workers`.",
            exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void GetArgumentValueByName()
    {
        var parseResult = _command.Parse("analyze https://github.com/corgibytes/freshli-fixture-ruby-nokotest");

        Assert.Equal("https://github.com/corgibytes/freshli-fixture-ruby-nokotest",
            parseResult.GetArgumentValueByName<string>("repository-location"));
    }

    [Fact]
    public void GetArgumentValueByNameWhenValueIsMissing()
    {
        var parseResult = _command.Parse("analyze https://github.com/corgibytes/freshli-fixture-ruby-nokotest");

        var exception = Assert.Throws<ArgumentException>(() => parseResult.GetArgumentValueByName<string>("missing"));
        Assert.Equal("No argument was found with the name `missing`. Valid option names are `repository-location`.",
            exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }
}
