using System.CommandLine;
using System.Linq;
using FluentAssertions;

namespace Corgibytes.Freshli.Cli.Test.Common;

public static class TestHelpers
{
    public static void VerifyAlias<T>(string alias, ArgumentArity arity, bool allowMultipleArgumentsPerToken)
        where T : Command, new()
    {
        var command = new T();
        var option = command.Options.FirstOrDefault(x => x.Aliases.Contains(alias));
        option.Should().NotBeNull();
        // ReSharper disable once PossibleNullReferenceException
        option.AllowMultipleArgumentsPerToken.Should().Be(allowMultipleArgumentsPerToken);
        option.Arity.Should().BeEquivalentTo(arity);
    }
}
