using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class ComputeLibYearForPackageActivityTest
{
    [Fact]
    public void HandleComputesLibYearAndFiresLibYearComputedForPackageEvent()
    {
        var analysisId = Guid.NewGuid();
        var asOfDate = new DateTimeOffset(2021, 1, 29, 12, 30, 45, 0, TimeSpan.Zero);
        var configuration = new Mock<IConfiguration>();
        var repositoryId = "abcef123";
        var commitId = "becfec231";
        var historyStopData = new HistoryStopData(configuration.Object, repositoryId, commitId, asOfDate);
        var agentExecutablePath = "/path/to/agent/smith";
        var package = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
        var packageLibYear = new PackageLibYear(
            asOfDate,
            package,
            asOfDate,
            package,
            6.2,
            asOfDate
        );

        var historyStopPointId = 29;
        var activity = new ComputeLibYearForPackageActivity
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            AgentExecutablePath = agentExecutablePath,
            Package = package
        };

        var eventClient = new Mock<IApplicationEventEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var calculator = new Mock<IPackageLibYearCalculator>();
        var agentManager = new Mock<IAgentManager>();
        var agentReader = new Mock<IAgentReader>();

        agentManager.Setup(mock => mock.GetReader(agentExecutablePath)).Returns(agentReader.Object);
        calculator.Setup(mock => mock.ComputeLibYear(agentReader.Object, package, asOfDate)).Returns(packageLibYear);

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IPackageLibYearCalculator))).Returns(calculator.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);

        activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<LibYearComputedForPackageEvent>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.AgentExecutablePath == agentExecutablePath &&
            value.PackageLibYear == packageLibYear)));
    }
}
