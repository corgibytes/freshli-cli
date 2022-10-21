using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Services;

[UnitTest]
public class AgentReaderTest
{
    [Fact]
    public void RetrieveReleaseHistoryWritesToCache()
    {
        var agentExecutable = "/path/to/agent";
        var commandInvoker = new Mock<ICommandInvoker>();
        var packageUrl = new PackageURL("pkg:maven/org.example/package");

        var alphaPackage = new Package(
            new PackageURL("pkg:maven/org.example/package@1"),
            new DateTimeOffset(2021, 12, 13, 14, 15, 16, TimeSpan.FromHours(-4)));
        var betaPackage = new Package(
            new PackageURL("pkg:maven/org.example/package@2"),
            alphaPackage.ReleasedAt.AddMonths(1));
        var gammaPackage = new Package(
            new PackageURL("pkg:maven/org.example/package@3"),
            alphaPackage.ReleasedAt.AddMonths(2));

        var expectedPackages = new List<Package>()
        {
            alphaPackage,
            betaPackage,
            gammaPackage
        };

        var commandResponse =
            $"{alphaPackage.PackageUrl.Version}\t{alphaPackage.ReleasedAt:yyyy'-'MM'-'dd'T'HH':'mm':'ssK}\n" +
            $"{betaPackage.PackageUrl.Version}\t{betaPackage.ReleasedAt:yyyy'-'MM'-'dd'T'HH':'mm':'ssK}\n" +
            $"{gammaPackage.PackageUrl.Version}\t{gammaPackage.ReleasedAt:yyyy'-'MM'-'dd'T'HH':'mm':'ssK}\n";

        commandInvoker.Setup(mock => mock.Run(agentExecutable,
            $"retrieve-release-history {packageUrl.FormatWithoutVersion()}", ".")).Returns(commandResponse);

        var reader = new AgentReader(commandInvoker.Object, agentExecutable);

        var retrievedPackages = reader.RetrieveReleaseHistory(packageUrl);

        Assert.Equal(expectedPackages, retrievedPackages);
    }
}
