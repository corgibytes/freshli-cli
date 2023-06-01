using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public interface IHistoryStopPointProcessingTask: IApplicationTask
{
    IHistoryStopPointProcessingTask Parent { get; }
    int HistoryStopPointId => Parent.HistoryStopPointId;
}
