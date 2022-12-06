using System;
using System.Diagnostics.CodeAnalysis;
using ShellProgressBar;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ShellProgressBarAnalyzeProgressReporter : IAnalyzeProgressReporter, IDisposable
{
    private readonly object _mainProgressBarLock = new();
    private ProgressBar? _mainProgressBar;

    private readonly object _gitOperationReporterLock = new();
    private ChildIndeterminateProgressReporter? _gitOperationReporter;

    private readonly object _historyStopPointDetectionReporterLock = new();
    private ChildIndeterminateProgressReporter? _historyStopPointDetectionReporter;

    private readonly object _historyStopPointArchivingReporterLock = new();
    private ChildProgressReporter? _historyStopPointArchivingReporter;

    private readonly object _historyStopPointProcessingReporterLock = new();
    private ChildProgressReporter? _historyStopPointProcessingReporter;

    private class ChildIndeterminateProgressReporter : IDisposable
    {
        private readonly string _message;
        private readonly ProgressBar _parentProgressBar;

        private readonly object _progressBarInitLock = new();
        private IndeterminateChildProgressBar? _progressBar;

        public ChildIndeterminateProgressReporter(ProgressBar parentProgressBar, string message)
        {
            _parentProgressBar = parentProgressBar;
            _message = message;
        }

        public void Start()
        {
            EnsureProgressBar();
        }

        public void Finish()
        {
            EnsureProgressBar();
            _progressBar.Finished();
        }

        [MemberNotNull(nameof(_progressBar))]
        private void EnsureProgressBar()
        {
            lock (_progressBarInitLock)
            {
                _progressBar ??= _parentProgressBar.SpawnIndeterminate(_message);
            }
        }

        public void Dispose()
        {
            _progressBar?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    private class ChildProgressReporter : IDisposable
    {
        private readonly object _progressBarInitLock = new();
        private int _tickCount;

        private readonly ProgressBar _parentProgressBar;
        private ChildProgressBar? _progressBar;

        private readonly string _message;

        public ChildProgressReporter(ProgressBar parentProgressBar, string message)
        {
            _parentProgressBar = parentProgressBar;
            _message = message;
        }

        public void Dispose()
        {
            _progressBar?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void OperationTasksStarted(int count)
        {
            lock (_progressBarInitLock)
            {
                _progressBar = _parentProgressBar.Spawn(count, _message);

                for (var _ = 0; _ < _tickCount; _++)
                {
                    _progressBar.Tick();
                }
            }
        }

        public void SingleOperationTaskCompleted()
        {
            lock (_progressBarInitLock)
            {
                _tickCount++;
                _progressBar?.Tick();
            }
        }
    }

    // this should be equal to the number of child progress bars that we're creating
    private const int WorkflowSteps = 4;

    [MemberNotNull(nameof(_mainProgressBar))]
    private void EnsureMainProgressBarInitialized()
    {
        lock (_mainProgressBarLock)
        {
            // TODO: Should we append the project url after "Analyzing"?
            _mainProgressBar ??= new ProgressBar(WorkflowSteps, "Analyzing");
        }
    }

    public void ReportGitOperationStarted(GitOperation operation)
    {
        EnsureGitOperationProgressReporterInitialized(operation);
        _gitOperationReporter.Start();
    }

    [MemberNotNull(nameof(_gitOperationReporter))]
    private void EnsureGitOperationProgressReporterInitialized(GitOperation operation)
    {
        EnsureMainProgressBarInitialized();

        // TODO: Should this include the repository URL?
        var progressMessage = "Cloning repository";
        if (operation == GitOperation.VerifyExistingClone)
        {
            // TODO: Should this include the path to the dir that's being verified?
            progressMessage = "Verifying local git repository";
        }

        lock (_gitOperationReporterLock)
        {
            _gitOperationReporter ??= new ChildIndeterminateProgressReporter(_mainProgressBar, progressMessage);
        }
    }

    public void ReportGitOperationFinished(GitOperation operation)
    {
        EnsureGitOperationProgressReporterInitialized(operation);
        _gitOperationReporter.Finish();
    }

    public void ReportHistoryStopPointDetectionStarted()
    {
        EnsureHistoryStopPointDetectionProgressBarInitialized();
        _historyStopPointDetectionReporter.Start();
    }

    [MemberNotNull(nameof(_historyStopPointDetectionReporter))]
    private void EnsureHistoryStopPointDetectionProgressBarInitialized()
    {
        EnsureMainProgressBarInitialized();

        lock (_historyStopPointDetectionReporterLock)
        {
            _historyStopPointDetectionReporter ??=
                new ChildIndeterminateProgressReporter(_mainProgressBar, "Detecting history stop points");
        }
    }

    public void ReportHistoryStopPointDetectionFinished()
    {
        EnsureHistoryStopPointDetectionProgressBarInitialized();
        _historyStopPointDetectionReporter.Finish();
    }

    public void ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation operation, int count)
    {
        var reporter = EnsureHistoryStopPointOperationReporter(operation);
        reporter.OperationTasksStarted(count);
    }

    public void ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation operation)
    {
        var reporter = EnsureHistoryStopPointOperationReporter(operation);
        reporter.SingleOperationTaskCompleted();
    }

    private ChildProgressReporter EnsureHistoryStopPointOperationReporter(HistoryStopPointOperation operation)
    {
        EnsureMainProgressBarInitialized();

        if (operation == HistoryStopPointOperation.Archive)
        {
            lock (_historyStopPointArchivingReporterLock)
            {
                _historyStopPointArchivingReporter ??= new ChildProgressReporter(_mainProgressBar,
                    "Checking out source for history stop points");
            }

            return _historyStopPointArchivingReporter;
        }

        lock (_historyStopPointProcessingReporterLock)
        {
            _historyStopPointProcessingReporter ??=
                new ChildProgressReporter(_mainProgressBar, "Processing history stop points");
        }

        return _historyStopPointProcessingReporter;
    }

    public void Dispose()
    {
        _gitOperationReporter?.Dispose();
        _historyStopPointDetectionReporter?.Dispose();
        _historyStopPointArchivingReporter?.Dispose();
        _historyStopPointProcessingReporter?.Dispose();

        _mainProgressBar?.Dispose();

        GC.SuppressFinalize(this);
    }
}
