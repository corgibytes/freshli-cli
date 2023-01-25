using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

public class NoManifestsDetectedEventTest
{
    private readonly Mock<IApplicationActivityEngine> _activityClient = new();
    private readonly Guid _analysisId = Guid.NewGuid();
    private const int HistoryStopPointId = 43;
    private readonly NoManifestsDetectedEvent _appEvent;

    public NoManifestsDetectedEventTest()
    {
        _appEvent = new NoManifestsDetectedEvent(_analysisId, HistoryStopPointId);
    }

    [Fact(Timeout = 500)]
    public async Task Handle()
    {
        await _appEvent.Handle(_activityClient.Object);

        _activityClient.Verify(mock => mock.Dispatch(
            It.Is<ReportHistoryStopPointProgressActivity>(value =>
                value.HistoryStopPointId == HistoryStopPointId)));
    }

    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyReportsExceptions()
    {
        var exception = new Exception();
        _activityClient.Setup(mock =>
            mock.Dispatch(It.IsAny<ReportHistoryStopPointProgressActivity>())
        )
        .Throws(exception);

        await _appEvent.Handle(_activityClient.Object);

        _activityClient.Verify(mock =>
            mock.Dispatch(It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                value.HistoryStopPointId == HistoryStopPointId &&
                value.Error == exception
            )
        ));
    }
}
