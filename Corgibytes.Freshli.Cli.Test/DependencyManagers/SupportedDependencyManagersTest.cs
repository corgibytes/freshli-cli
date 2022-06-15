using System;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.DependencyManagers;

public class SupportedDependencyManagersTest : FreshliTest
{
    public SupportedDependencyManagersTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void It_can_not_instantiate_with_invalid_dependency_manager()
    {
        ArgumentException caughtException =
            Assert.Throws<ArgumentException>(() =>
                SupportedDependencyManagers.FromString("prettysurethiscanneverwork"));

        Assert.Equal("Invalid dependency manager given 'prettysurethiscanneverwork'", caughtException.Message);
    }
}

