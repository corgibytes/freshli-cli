using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Common
{
    public class FreshliTest
    {
        protected ITestOutputHelper Output { get; private set; }

        public FreshliTest(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}
