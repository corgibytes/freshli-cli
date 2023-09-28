using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Api;

[UnitTest]
public class UpdateAnalysisStatusActivityTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
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

        var cancellationToken = new System.Threading.CancellationToken(false);
        await activity.Handle(eventClient.Object, cancellationToken);

        resultsApi.Verify(mock => mock.UpdateAnalysis(apiAnalysisId, status));

        eventClient.Verify(mock =>
            mock.Fire(
                It.Is<AnalysisApiStatusUpdatedEvent>(value =>
                    value.ApiAnalysisId == apiAnalysisId &&
                    value.Status == status
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
