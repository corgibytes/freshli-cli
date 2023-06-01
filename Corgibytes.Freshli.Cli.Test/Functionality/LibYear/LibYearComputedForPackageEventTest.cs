using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class LibYearComputedForPackageEventTest
{
    private const int PackageLibYearId = 9;
    private const string AgentExecutablePath = "/path/to/agent";
    private readonly Guid _analysisId = Guid.NewGuid();
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new Mock<IHistoryStopPointProcessingTask>();
    private readonly CancellationToken _cancellationToken = new System.Threading.CancellationToken(false);
    private readonly LibYearComputedForPackageEvent _appEvent;
    private readonly Mock<IApplicationActivityEngine> _activityClient = new Mock<IApplicationActivityEngine>();

    public LibYearComputedForPackageEventTest()
    {
        _appEvent = new LibYearComputedForPackageEvent
        {
            AnalysisId = _analysisId,
            Parent = _parent.Object,
            PackageLibYearId = PackageLibYearId,
            AgentExecutablePath = AgentExecutablePath
        };
    }

    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyDispatchesCreateApiPackageLibYear()
    {
        await _appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.Verify(mock =>
            mock.Dispatch(
                It.Is<CreateApiPackageLibYearActivity>(value =>
                    value.AnalysisId == _analysisId &&
                    value.Parent == _parent.Object &&
                    value.PackageLibYearId == PackageLibYearId &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = 500)]
    public async Task HandleDispatchesFireHistoryStopPointProcessingErrorActivity()
    {
        var exception = new InvalidOperationException();
        _activityClient.Setup(mock =>
            mock.Dispatch(
                It.IsAny<CreateApiPackageLibYearActivity>(),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        ).Throws(exception);

        await _appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.Verify(mock =>
            mock.Dispatch(
                It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                    value.Parent == _appEvent.Parent &&
                    value.Error == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
