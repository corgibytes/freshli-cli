using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Commands
{
    public class CacheCommandTest : FreshliTest
    {
        public CacheCommandTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Verify_no_cache_handler_configuration()
        {
            CacheCommand cacheCommand = new();
            cacheCommand.Handler.Should().BeNull();
        }

        [Fact]
        public void Verify_prepare_handler_configuration()
        {
            CachePrepareCommand cachePrepareCommand = new();
            cachePrepareCommand.Handler.Should().NotBeNull();
        }
    }
}
