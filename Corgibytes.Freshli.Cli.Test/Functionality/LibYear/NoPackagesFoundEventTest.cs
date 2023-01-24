using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

public class NoPackagesFoundEventTest
{
    private readonly Mock<IApplicationActivityEngine> _activityEngine = new();
    private readonly Guid _analysisId = Guid.NewGuid();
    private const int HistoryStopPointId = 33;
    private NoPackagesFoundEvent _appEvent;

    public NoPackagesFoundEventTest()
    {
        _appEvent = new NoPackagesFoundEvent(_analysisId, HistoryStopPointId);
    }

    [Fact(Timeout = 500)]
    public async Task Handle()
    {
        await _appEvent.Handle(_activityEngine.Object);

        _activityEngine.Verify(mock =>
            mock.Dispatch(It.Is<ReportHistoryStopPointProgressActivity>(value =>
                value.HistoryStopPointId == HistoryStopPointId
            )
        ));
    }

    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var exception = new Exception();

        _activityEngine.Setup(mock =>
            mock.Dispatch(It.IsAny<ReportHistoryStopPointProgressActivity>())
        )
        .Throws(exception);

        await _appEvent.Handle(_activityEngine.Object);

        _activityEngine.Verify(mock =>
            mock.Dispatch(It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                    value.HistoryStopPointId == HistoryStopPointId &&
                    value.Error == exception
            )
        ));
    }
}
