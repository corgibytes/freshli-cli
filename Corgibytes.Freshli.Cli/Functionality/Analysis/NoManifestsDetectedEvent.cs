using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class NoManifestsDetectedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }

    public NoManifestsDetectedEvent(Guid analysisId, int historyStopPointId)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
    }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        try
        {
            await eventClient.Dispatch(
                new ReportHistoryStopPointProgressActivity { HistoryStopPointId = HistoryStopPointId });
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(new FireHistoryStopPointProcessingErrorActivity(HistoryStopPointId, error));
        }
    }
}
