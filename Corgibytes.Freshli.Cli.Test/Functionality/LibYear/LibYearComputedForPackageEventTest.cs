using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class LibYearComputedForPackageEventTest
{
    [Fact]
    public void HandleCorrectlyDispatchesCreateApiPackageLibYear()
    {
        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 12;
        const int packageLibYearId = 9;
        const string agentExecutablePath = "/path/to/agent";

        // note: these dates are completely fabricated
        var releaseDateCurrentVersion = new DateTimeOffset(2021, 12, 23, 11, 22, 33, 44, TimeSpan.Zero);
        var currentVersion =
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease");
        var releaseDateLatestVersion = new DateTimeOffset(2021, 12, 24, 11, 22, 33, 44, TimeSpan.Zero);
        var latestVersion =
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.10?repository_url=repo.spring.io%2Frelease");
        const double libYear = 1.2;
        var asOfDateTime = new DateTimeOffset(2021, 12, 25, 11, 22, 33, 44, TimeSpan.Zero);

        var packageLibYear = new PackageLibYear(releaseDateCurrentVersion, currentVersion, releaseDateLatestVersion,
            latestVersion, libYear, asOfDateTime);

        var appEvent = new LibYearComputedForPackageEvent
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            PackageLibYearId = packageLibYearId,
            AgentExecutablePath = agentExecutablePath
        };

        var activityClient = new Mock<IApplicationActivityEngine>();

        appEvent.Handle(activityClient.Object);

        activityClient.Verify(mock => mock.Dispatch(It.Is<CreateApiPackageLibYearActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.PackageLibYearId == packageLibYearId &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }
}
