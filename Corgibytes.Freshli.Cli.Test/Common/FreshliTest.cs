using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Common;

public class FreshliTest
{
    public FreshliTest(ITestOutputHelper output) => Output = output;

    protected ITestOutputHelper Output { get; }
}
