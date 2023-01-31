using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class AgentReaderWithJavaAgentTest : IDisposable
{
    private readonly AgentManager _agentManager;
    public AgentReaderWithJavaAgentTest()
    {
        _agentManager = new AgentManager(
            new CacheManager(new Configuration(new Environment())),
            new NullLogger<AgentManager>(),
            new Configuration(new Environment())
        );
    }

    [Fact]
    public async Task DetectManifestsUsingProtobuf()
    {
        SetupDirectory(out var repositoryLocation, out var reader, out var checkoutDirectory);

        var actualManifests = await reader.DetectManifests(repositoryLocation).ToListAsync();

        var expectedManifests = new List<string>
        {
            Path.GetFullPath(Path.Combine("java","pom.xml"), repositoryLocation),
            Path.GetFullPath(Path.Combine("java", "protoc", "pom.xml"), repositoryLocation),
            Path.GetFullPath(Path.Combine("ruby", "pom.xml"), repositoryLocation)
        };
        Assert.Equal(expectedManifests, actualManifests);

        // delete cloned files
        RecursiveDelete(checkoutDirectory);
    }

    [Fact]
    public async Task GenerateBillOfMaterialsUsingProtobuf()
    {
        SetupDirectory(out var repositoryLocation, out var reader, out var checkoutDirectory);

        // java/pom.xml is detected by detect manifest, see also DetectManifestsUsingProtobuf()
        var billOfMaterialsPath =
            await reader.ProcessManifest(Path.Combine(repositoryLocation, "java", "pom.xml"), DateTime.Now);

        Assert.Equal(Path.Combine(repositoryLocation, "java", "target", "bom.json"), billOfMaterialsPath);

        // delete cloned files
        RecursiveDelete(checkoutDirectory);
    }

    [Fact]
    public async Task AgentReaderReturnsEmptyListWhenNoManifestsFound()
    {
        var checkoutLocation = CreateCheckoutLocation(out var checkoutDirectory);

        var reader = _agentManager.GetReader("freshli-agent-java");
        var repositoryLocation = Path.Combine(checkoutLocation, "invalid_repository");

        var actualManifests = await reader.DetectManifests(repositoryLocation).ToListAsync();
        Assert.Empty(actualManifests);
        RecursiveDelete(checkoutDirectory);
    }

    private void SetupDirectory(out string repositoryLocation, out IAgentReader reader,
        out DirectoryInfo checkoutDirectory)
    {
        var checkoutLocation = CreateCheckoutLocation(out checkoutDirectory);

        // clone https://github.com/protocolbuffers/protobuf to a temp location
        new CommandInvoker()
            .Run("git", "clone https://github.com/protocolbuffers/protobuf", checkoutLocation).AsTask().Wait();

        repositoryLocation = Path.Combine(checkoutLocation, "protobuf");

        reader = _agentManager.GetReader("freshli-agent-java");
    }

    private static string CreateCheckoutLocation(out DirectoryInfo checkoutDirectory)
    {
        var checkoutLocation = Path.Combine(Path.GetTempPath(), "repositories");

        checkoutDirectory = new DirectoryInfo(checkoutLocation);
        if (checkoutDirectory.Exists)
        {
            RecursiveDelete(checkoutDirectory);
        }

        checkoutDirectory.Create();
        return checkoutLocation;
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
