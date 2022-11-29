using System;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public interface IAnalyzeProgressReporter
{
    void ReportGitOperationStarted(GitOperation operation);
    void ReportGitOperationFinished(GitOperation operation);

    IProgress<int> ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation operation, int count);
    void ReportHistoryStopPointOperationFinished(HistoryStopPointOperation operation);
}
