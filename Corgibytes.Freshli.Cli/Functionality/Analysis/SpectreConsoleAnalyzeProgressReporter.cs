﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Resources;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class SpectreConsoleAnalyzeProgressReporter : IAnalyzeProgressReporter, IDisposable
{
    private readonly IAnsiConsole _console;

    private readonly object _mainProgressBarLock = new();
    private Progress? _mainProgressBar;
    private Task? _mainProgressBarTask;
    private ProgressContext? _mainProgressBarContext;
    private readonly ManualResetEventSlim _mainProgressBarKeepAlive = new();

    private readonly object _gitOperationLock = new();
    private ProgressTask? _gitOperationProgressTask;

    private readonly object _historyStopPointDetectionLock = new();
    private ProgressTask? _historyStopPointDetectionProgressTask;

    private readonly object _historyStopPointArchiveLock = new();
    private ProgressTask? _historyStopPointArchiveProgressTask;

    private readonly object _historyStopPointProcessingLock = new();
    private ProgressTask? _historyStopPointProcessingProgressTask;
    private int _historyStopPointProcessingTickCount;

    public SpectreConsoleAnalyzeProgressReporter(IAnsiConsole console)
    {
        _console = console;
    }

    [MemberNotNull(nameof(_mainProgressBar))]
    [MemberNotNull(nameof(_mainProgressBarTask))]
    [MemberNotNull(nameof(_mainProgressBarContext))]
    private void EnsureMainProgressBarInitialized()
    {
        lock (_mainProgressBarLock)
        {
            if (_mainProgressBar != null && _mainProgressBarTask != null && _mainProgressBarContext != null)
            {
                return;
            }

            _mainProgressBar = _console.Progress();
            _mainProgressBar.Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new DecimalPercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn()
            );
            _mainProgressBarTask = _mainProgressBar.StartAsync(async (context) =>
            {
                _mainProgressBarContext = context;
                await Task.Run(() => _mainProgressBarKeepAlive.Wait());
            });

            Task.Run(async () =>
            {
                while (_mainProgressBarContext == null)
                {
                    await Task.Delay(10);
                }
            }).Wait();
        }

        if (_mainProgressBarContext == null)
        {
            throw new InvalidOperationException("progress bar context was not set");
        }
    }

    [MemberNotNull(nameof(_gitOperationProgressTask))]
    private void EnsureGitOperationProgressTaskInitialized(GitOperation operation)
    {
        EnsureMainProgressBarInitialized();

        var progressMessage = "Cloning repository";
        if (operation == GitOperation.VerifyExistingClone)
        {
            progressMessage = "Verifying local git repository";
        }

        lock (_gitOperationLock)
        {
            if (_gitOperationProgressTask != null)
            {
                return;
            }

            _gitOperationProgressTask = _mainProgressBarContext.AddTask(progressMessage);
            _gitOperationProgressTask.IsIndeterminate = true;
        }
    }

    public void ReportGitOperationStarted(GitOperation operation)
    {
        EnsureGitOperationProgressTaskInitialized(operation);
    }

    public void ReportGitOperationFinished(GitOperation operation)
    {
        EnsureGitOperationProgressTaskInitialized(operation);
        _gitOperationProgressTask.Increment(_gitOperationProgressTask.MaxValue);
        _gitOperationProgressTask.StopTask();
    }

    [MemberNotNull(nameof(_historyStopPointDetectionProgressTask))]
    private void EnsureHistoryStopPointDetectionProgressTaskInitialized()
    {
        EnsureMainProgressBarInitialized();

        lock (_historyStopPointDetectionLock)
        {
            if (_historyStopPointDetectionProgressTask != null)
            {
                return;
            }

            _historyStopPointDetectionProgressTask = _mainProgressBarContext.AddTask("Detecting history stop points");
            _historyStopPointDetectionProgressTask.IsIndeterminate = true;
        }
    }

    public void ReportHistoryStopPointDetectionStarted()
    {
        EnsureHistoryStopPointDetectionProgressTaskInitialized();
    }

    public void ReportHistoryStopPointDetectionFinished()
    {
        EnsureHistoryStopPointDetectionProgressTaskInitialized();
        _historyStopPointDetectionProgressTask.Increment(_historyStopPointDetectionProgressTask.MaxValue);
        _historyStopPointDetectionProgressTask.StopTask();
    }

    private void EnsureHistoryStopPointAnalysisProgressTaskInitialize(int count)
    {
        EnsureMainProgressBarInitialized();

        lock (_historyStopPointArchiveLock)
        {
            if (_historyStopPointArchiveProgressTask != null)
            {
                return;
            }

            _historyStopPointArchiveProgressTask =
                _mainProgressBarContext.AddTask("Checking out source for history stop points");
            _historyStopPointArchiveProgressTask.Value = _historyStopPointProcessingTickCount;
            _historyStopPointArchiveProgressTask.MaxValue = count;
        }
    }

    private void EnsureHistoryStopPointProcessingProgressTaskInitialized(int count)
    {
        EnsureMainProgressBarInitialized();

        lock (_historyStopPointProcessingLock)
        {
            if (_historyStopPointProcessingProgressTask != null)
            {
                return;
            }

            _historyStopPointProcessingProgressTask = _mainProgressBarContext.AddTask("Processing history stop points");
            _historyStopPointProcessingProgressTask.Value = _historyStopPointProcessingTickCount;
            _historyStopPointProcessingProgressTask.MaxValue = count;
        }
    }

    public void ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation operation, int count)
    {
        switch (operation)
        {
            case HistoryStopPointOperation.Archive:
                EnsureHistoryStopPointAnalysisProgressTaskInitialize(count);
                break;

            case HistoryStopPointOperation.Process:
                EnsureHistoryStopPointProcessingProgressTaskInitialized(count);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(operation),
                    operation,
                    CliOutput.SpectreConsoleAnalyzeProgressReporter_ReportHistoryStopPointsOperationStarted_Unexpected_HistoryStopPointOperation_value
                );
        }
    }

    public void ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation operation)
    {
        switch (operation)
        {
            case HistoryStopPointOperation.Archive:
                lock (_historyStopPointArchiveLock)
                {
                    if (_historyStopPointArchiveProgressTask != null)
                    {
                        _historyStopPointArchiveProgressTask.Increment(1);
                        if (_historyStopPointArchiveProgressTask.IsFinished)
                        {
                            _historyStopPointArchiveProgressTask.StopTask();
                        }
                    }
                }
                break;

            case HistoryStopPointOperation.Process:
                lock (_historyStopPointProcessingLock)
                {
                    _historyStopPointProcessingTickCount++;
                    if (_historyStopPointProcessingProgressTask != null)
                    {
                        _historyStopPointProcessingProgressTask.Increment(1);
                        if (_historyStopPointProcessingProgressTask.IsFinished)
                        {
                            _historyStopPointProcessingProgressTask.StopTask();
                        }
                    }
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(operation),
                    operation,
                    CliOutput.SpectreConsoleAnalyzeProgressReporter_ReportHistoryStopPointsOperationStarted_Unexpected_HistoryStopPointOperation_value
                );
        }
    }

    public void Dispose()
    {
        _mainProgressBarKeepAlive.Set();
        try
        {
            _mainProgressBarTask?.Wait();
        }
        catch (AggregateException)
        {
            // TODO: Make this catch a little more robust. Even better. Figure out how to prevent this exception from
            // being thrown in the first place
        }

        _mainProgressBarTask?.Dispose();
        _mainProgressBar = null;

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// A column showing task progress in percentage.
    /// </summary>
    private sealed class DecimalPercentageColumn : ProgressColumn
    {
        /// <summary>
        /// Gets or sets the style for a non-complete task.
        /// </summary>
        private static Style Style => Style.Plain;

        /// <summary>
        /// Gets or sets the style for a completed task.
        /// </summary>
        private static Style CompletedStyle => Color.Green;

        /// <inheritdoc/>
        public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
        {
            const double tolerance = 0.000001;
            var percentage = Math.Min(task.Percentage, 100.0);
            var style = Math.Abs(percentage - 100.0) < tolerance ? CompletedStyle : Style;

            var formattedPercentage = $"{percentage:0.00}";
            if (formattedPercentage.EndsWith(".00"))
            {
                formattedPercentage = $"{percentage:0}";
            }
            return new Text($"{formattedPercentage}%", style).RightJustified();
        }

        /// <inheritdoc/>
        public override int? GetColumnWidth(RenderOptions options)
        {
            return 6;
        }
    }
}
