using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class LibYearComputedForPackageEventTest
{
    private readonly CachedPackageLibYear _packageLibYear = new() { Id = 9 };
    private const string AgentExecutablePath = "/path/to/agent";
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly CancellationToken _cancellationToken = new(false);
    private readonly LibYearComputedForPackageEvent _appEvent;
    private readonly Mock<IApplicationActivityEngine> _activityClient = new();

    public LibYearComputedForPackageEventTest()
    {
        _appEvent = new LibYearComputedForPackageEvent
        {
            Parent = _parent.Object,
            PackageLibYear = _packageLibYear,
            AgentExecutablePath = AgentExecutablePath
        };
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyDispatchesCreateApiPackageLibYear()
    {
        await _appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.Verify(mock =>
            mock.Dispatch(
                It.Is<CreateApiPackageLibYearActivity>(value =>
                    value.Parent == _appEvent &&
                    value.PackageLibYear == _packageLibYear &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
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
                    value.Parent == _appEvent &&
                    value.Error == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
