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
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var activity = new FireHistoryStopPointProcessingErrorActivity(parent.Object, new InvalidOperationException());

        var eventClient = new Mock<IApplicationEventEngine>();

        var cancellationToken = new System.Threading.CancellationToken(false);
        await activity.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(mock =>
            mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == activity.Parent &&
                    value.Exception == activity.Error
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
