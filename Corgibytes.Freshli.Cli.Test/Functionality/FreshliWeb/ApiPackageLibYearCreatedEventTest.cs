using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class ApiPackageLibYearCreatedEventTest
{
    private const string AgentExecutablePath = "/path/to/agent";
    private const int PackageLibYearId = 12;
    private readonly Guid _analysisId = Guid.NewGuid();
    private readonly CancellationToken _cancellationToken = new(false);
    private readonly Mock<IApplicationActivityEngine> _activityClient = new();
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();

    private readonly ApiPackageLibYearCreatedEvent _appEvent;

    public ApiPackageLibYearCreatedEventTest()
    {
        _appEvent = new ApiPackageLibYearCreatedEvent
        {
            AnalysisId = _analysisId,
            Parent = _parent.Object,
            AgentExecutablePath = AgentExecutablePath,
            PackageLibYearId = PackageLibYearId
        };
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        await _appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.VerifyNoOtherCalls();
    }
}
