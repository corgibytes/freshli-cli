using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Common;

public class FreshliTest
{
    protected FreshliTest(ITestOutputHelper output) => Output = output;

    protected ITestOutputHelper Output { get; }
}
