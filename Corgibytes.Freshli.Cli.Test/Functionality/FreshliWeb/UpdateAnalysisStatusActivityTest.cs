using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class UpdateAnalysisStatusActivityTest
{
    [Fact]
    public async Task HandleSendsSuccessStatusToApi()
    {
        var apiAnalysisId = Guid.NewGuid();
        const string status = "success";

        var activity = new UpdateAnalysisStatusActivity(apiAnalysisId, status);

        var eventClient = new Mock<IApplicationEventEngine>();

        var serviceProvider = new Mock<IServiceProvider>();
        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var resultsApi = new Mock<IResultsApi>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(resultsApi.Object);

        await activity.Handle(eventClient.Object);

        resultsApi.Verify(mock => mock.UpdateAnalysis(apiAnalysisId, status));

        eventClient.Verify(mock => mock.Fire(It.Is<AnalysisApiStatusUpdatedEvent>(value =>
            value.ApiAnalysisId == apiAnalysisId &&
            value.Status == status
        )));
    }
}
