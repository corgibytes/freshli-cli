using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class AgentReaderTest
{
    [Fact]
    public void DetectManifestsUsingProtobuf()
    {
        var checkoutLocation = Path.Combine(Path.GetTempPath(), "repositories");

        var checkoutDirectory = new DirectoryInfo(checkoutLocation);
        if (checkoutDirectory.Exists)
        {
            checkoutDirectory.Delete(true);
        }
        checkoutDirectory.Create();

        // clone https://github.com/protocolbuffers/protobuf to a temp location
        Invoke.Command("git", "clone https://github.com/protocolbuffers/protobuf", checkoutLocation);

        var repositoryLocation = Path.Combine(checkoutLocation, "protobuf");

        var reader = new AgentReader("freshli-agent-java");

        var actualManifests = reader.DetectManifests(repositoryLocation);

        var expectedManifests = new List<string>()
        {
            "java/pom.xml",
            "java/protoc/pom.xml",
            "protoc-artifacts/pom.xml",
            "ruby/pom.xml"
        };
        Assert.Equal(expectedManifests, actualManifests);

        // delete cloned files
        checkoutDirectory.Delete(true);
    }
}
