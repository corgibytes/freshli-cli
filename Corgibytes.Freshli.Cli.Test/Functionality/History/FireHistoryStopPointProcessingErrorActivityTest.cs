using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class FireHistoryStopPointProcessingErrorActivityTest
{
    [Fact]
    public async Task Handle()
    {
        var activity = new FireHistoryStopPointProcessingErrorActivity(12, new InvalidOperationException());

        var eventClient = new Mock<IApplicationEventEngine>();

        await activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<HistoryStopPointProcessingFailedEvent>(value =>
            value.HistoryStopPointId == activity.HistoryStopPointId &&
            value.Exception == activity.Error)));
    }
}
