using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
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
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
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

        var expectedPackages = new List<Package>
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

        var initialCachedPackages = new List<CachedPackage>();

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveCachedReleaseHistory(packageUrl)).Returns(initialCachedPackages);

        var reader = new AgentReader(cacheManager.Object, commandInvoker.Object, agentExecutable);

        var retrievedPackages = reader.RetrieveReleaseHistory(packageUrl);

        Assert.Equal(expectedPackages, retrievedPackages);

        cacheDb.Verify(mock => mock.StoreCachedReleaseHistory(It.Is<List<CachedPackage>>(value =>
            value.Count == 3 &&
            value[0].ToPackage().Equals(alphaPackage) &&
            value[1].ToPackage().Equals(betaPackage) &&
            value[2].ToPackage().Equals(gammaPackage)
        )));
    }

    [Fact]
    public void RetrieveReleaseHistoryReadsFromCache()
    {
        var agentExecutable = "/path/to/agent";
        var commandInvoker = new Mock<ICommandInvoker>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
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

        var expectedPackages = new List<Package>
        {
            alphaPackage,
            betaPackage,
            gammaPackage
        };

        var initialCachedPackages = new List<CachedPackage>
        {
            new(alphaPackage),
            new(betaPackage),
            new(gammaPackage)
        };

        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveCachedReleaseHistory(packageUrl)).Returns(initialCachedPackages);

        var reader = new AgentReader(cacheManager.Object, commandInvoker.Object, agentExecutable);

        var retrievedPackages = reader.RetrieveReleaseHistory(packageUrl);

        Assert.Equal(expectedPackages, retrievedPackages);
    }
}
