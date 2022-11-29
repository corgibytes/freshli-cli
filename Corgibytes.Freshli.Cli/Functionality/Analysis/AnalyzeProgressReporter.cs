using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ShellProgressBar;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalyzeProgressReporter : IAnalyzeProgressReporter, IDisposable
{
    private ProgressBar? _mainProgressBar;
    private IndeterminateChildProgressBar? _gitOperationProgressBar;
    private const int WorkflowSteps = 4;

    [MemberNotNull(nameof(_mainProgressBar))]
    private void EnsureMainProgressBarInitialized()
    {
        // TODO: Should we append the project url after "Analyzing"?
        _mainProgressBar = new ProgressBar(WorkflowSteps, "Analyzing");
    }

    public void ReportGitOperationStarted(GitOperation operation)
    {
        EnsureMainProgressBarInitialized();
        var progressMessage = "Cloning repository";
        if (operation == GitOperation.VerifyExistingClone)
        {
            progressMessage = "Verifying local git repository";
        }

        _gitOperationProgressBar = _mainProgressBar.SpawnIndeterminate(progressMessage);
    }

    public void ReportGitOperationFinished(GitOperation operation) => throw new NotImplementedException();

    public IProgress<int> ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation operation, int count) => throw new NotImplementedException();

    public void ReportHistoryStopPointOperationFinished(HistoryStopPointOperation operation) => throw new NotImplementedException();

    public void Dispose()
    {
        _gitOperationProgressBar?.Dispose();
        _mainProgressBar?.Dispose();

        GC.SuppressFinalize(this);
    }
}
