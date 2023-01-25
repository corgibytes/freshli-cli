namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public interface IAnalyzeProgressReporter
{
    void ReportGitOperationStarted(GitOperation operation);
    void ReportGitOperationFinished(GitOperation operation);

    void ReportHistoryStopPointDetectionStarted();
    void ReportHistoryStopPointDetectionFinished();

    void ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation operation, int count);
    void ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation operation);
}
