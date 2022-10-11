using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class LogAnalysisFailureActivityTest
{
    [Fact]
    public void VerifyItFiresAnalysisFailureLoggedEvent()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var eventClient = new Mock<IApplicationEventEngine>();
        var logger = new Logger<LogAnalysisFailureActivity>(new LoggerFactory());

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<LogAnalysisFailureActivity>)))
            .Returns(logger);

        var errorEvent = new AnalysisIdNotFoundEvent();
        var activity = new LogAnalysisFailureActivity(errorEvent);

        activity.Handle(eventClient.Object);

        eventClient.Setup(mock => mock.Fire(It.Is<AnalysisFailureLoggedEvent>(value =>
            value.ErrorEvent == errorEvent
        )));
    }
}

