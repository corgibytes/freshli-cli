using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryStopCheckedOutEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }

    public async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new DetectAgentsForDetectManifestsActivity(AnalysisId, HistoryStopPointId));
}
