using System;
using System.Threading;
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
    private readonly NoManifestsDetectedEvent _appEvent;
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly CancellationToken _cancellationToken = new(false);

    public NoManifestsDetectedEventTest()
    {
        _appEvent = new NoManifestsDetectedEvent
        {
            AnalysisId = _analysisId,
            Parent = _parent.Object
        };
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        await _appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.VerifyNoOtherCalls();
    }
}
