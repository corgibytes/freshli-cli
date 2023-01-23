using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class NoPackagesFoundEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }

    public NoPackagesFoundEvent(Guid analysisId, int historyStopPointId)
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
