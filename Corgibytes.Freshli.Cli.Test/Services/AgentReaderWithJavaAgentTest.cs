using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class AgentReaderWithJavaAgentTest : IDisposable
{
    private readonly AgentManager _agentManager;
    public AgentReaderWithJavaAgentTest()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<AgentReader>)))
            .Returns(NullLogger<AgentReader>.Instance);
        _agentManager = new AgentManager(
            new CacheManager(new Configuration(new Environment())),
            new NullLogger<AgentManager>(),
            new Configuration(new Environment()),
            serviceProvider.Object
        );
    }

    private const int MillisecondsPerMinute = 60 * 1000;
    private const int TenMinutes = 10 * MillisecondsPerMinute;

    [Fact(Timeout = TenMinutes)]
    public async Task DetectManifestsUsingProtobuf()
    {
        var (repositoryLocation, reader, checkoutDirectory) = await SetupDirectory();

        var actualManifests = await reader.DetectManifests(repositoryLocation).ToListAsync();

        var expectedManifests = new List<string>
        {
            Path.GetFullPath(Path.Combine("java", "pom.xml"), repositoryLocation),
            Path.GetFullPath(Path.Combine("java", "protoc", "pom.xml"), repositoryLocation),
            Path.GetFullPath(Path.Combine("ruby", "pom.xml"), repositoryLocation)
        };
        actualManifests.Should().Equal(expectedManifests);

        // delete cloned files
        RecursiveDelete(checkoutDirectory);
    }

    [Fact(Timeout = TenMinutes)]
    public async Task GenerateBillOfMaterialsUsingProtobuf()
    {
        var (repositoryLocation, reader, checkoutDirectory) = await SetupDirectory();

        // java/pom.xml is detected by detect manifest, see also DetectManifestsUsingProtobuf()
        var billOfMaterialsPath =
            await reader.ProcessManifest(Path.Combine(repositoryLocation, "java", "pom.xml"), DateTime.Now);

        Assert.Equal(Path.Combine(repositoryLocation, "java", "target", "bom.json"), billOfMaterialsPath);

        // delete cloned files
        RecursiveDelete(checkoutDirectory);
    }

    [Fact(Timeout = TenMinutes)]
    public async Task AgentReaderReturnsEmptyListWhenNoManifestsFound()
    {
        var (checkoutLocation, checkoutDirectory) = CreateCheckoutLocation();

        var reader = _agentManager.GetReader("freshli-agent-java");
        var repositoryLocation = Path.Combine(checkoutLocation, "invalid_repository");

        var actualManifests = await reader.DetectManifests(repositoryLocation).ToListAsync();
        Assert.Empty(actualManifests);
        RecursiveDelete(checkoutDirectory);
    }

    private async ValueTask<(string, IAgentReader, DirectoryInfo)> SetupDirectory()
    {
        var (checkoutLocation, _) = CreateCheckoutLocation();

        // clone https://github.com/protocolbuffers/protobuf to a temp location
        await new CommandInvoker()
            .Run("git", "clone https://github.com/protocolbuffers/protobuf", checkoutLocation);

        var repositoryLocation = Path.Combine(checkoutLocation, "protobuf");

        var reader = _agentManager.GetReader("freshli-agent-java");

        return (repositoryLocation, reader, new DirectoryInfo(repositoryLocation));
    }

    private static (string, DirectoryInfo) CreateCheckoutLocation()
    {
        var checkoutLocation = Path.Combine(Path.GetTempPath(), "repositories");

        var checkoutDirectory = new DirectoryInfo(checkoutLocation);
        if (checkoutDirectory.Exists)
        {
            RecursiveDelete(checkoutDirectory);
        }

        checkoutDirectory.Create();
        return (checkoutLocation, checkoutDirectory);
    }

    private static void RecursiveDelete(DirectoryInfo checkoutDirectory)
    {
        foreach (var file in checkoutDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
        {
            file.Attributes = FileAttributes.Normal;
        }

        foreach (var file in checkoutDirectory.EnumerateFileSystemInfos(".*", SearchOption.AllDirectories))
        {
            file.Attributes = FileAttributes.Normal;
        }

        checkoutDirectory.Delete(true);
    }

    public void Dispose()
    {
        _agentManager.Dispose();

        GC.SuppressFinalize(this);
    }
}
