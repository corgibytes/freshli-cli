using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Extensions;
using System.CommandLine;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Extensions;

[IntegrationTest]
public class ParseResultExtensionsTest
{
    [Fact]
    public void GetOptionValueByNameForCommandOption()
    {
        var command = new MainCommand();
        var parseResult = command.Parse("--loglevel debug git clone --git-path git");

        Assert.Equal("git", parseResult.GetOptionValueByName<string>("git-path"));
    }

    [Fact]
    public void GetOptionValueByNameForGlobalCommandOption()
    {
        var command = new MainCommand();
        var parseResult = command.Parse("--loglevel debug git clone --git-path git");

        Assert.Equal("debug", parseResult.GetOptionValueByName<string>("loglevel"));
    }

}
