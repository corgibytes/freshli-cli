using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class ComputeLibYearForBomActivityTest
{
    [Fact]
    public void HandleCorrectlyFiresLibYearComputatitonForBomStartedEvent()
    {
        var analysisId = Guid.NewGuid();
        var configuration = new Mock<IConfiguration>();
        var repositoryId = "abcdef123";
        var commitId = "fecdef987";
        var asOf = new DateTimeOffset(2022, 10, 07, 22, 02, 54, 0, TimeSpan.Zero);
        var pathToBom = "/path/to/bom";

        var historyStopData = new HistoryStopData(configuration.Object, repositoryId, commitId, asOf);

        var eventClient = new Mock<IApplicationEventEngine>();

        var activity = new ComputeLibYearForBomActivity(analysisId, historyStopData, pathToBom);

        var serviceProvider = new Mock<IServiceProvider>();

        var packageAlpha = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");

        var packageBeta = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0");

        // mock out a list of packages that are found by the IBomReader

        var bomReader = new Mock<IBomReader>();

        bomReader.Setup(mock => mock.AsPackageUrls(pathToBom)).Returns(new List<PackageURL>
        {
            packageAlpha,
            packageBeta
        });

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IBomReader))).Returns(bomReader.Object);

        activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<PackageFoundEvent>(value =>
            value.Package == packageAlpha
        )));

        eventClient.Verify(mock => mock.Fire(It.Is<PackageFoundEvent>(value =>
            value.Package == packageBeta
        )));
    }
}
