using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class LogAnalysisFailureActivityTest
{
    [Fact(Timeout = 500)]
    public async Task VerifyItFiresAnalysisFailureLoggedEvent()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var eventClient = new Mock<IApplicationEventEngine>();
        var logger = new Logger<LogAnalysisFailureActivity>(new LoggerFactory());

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<LogAnalysisFailureActivity>)))
            .Returns(logger);

        var errorEvent = new AnalysisIdNotFoundEvent();
        var cancellationToken = new System.Threading.CancellationToken(false);
        var activity = new LogAnalysisFailureActivity(errorEvent);

        await activity.Handle(eventClient.Object, cancellationToken);

        eventClient.Setup(
            mock => mock.Fire(
                It.Is<AnalysisFailureLoggedEvent>(value =>
                    value.ErrorEvent == errorEvent
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
