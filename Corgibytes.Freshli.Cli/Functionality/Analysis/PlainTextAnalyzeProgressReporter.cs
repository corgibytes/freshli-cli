using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class PlainTextAnalyzeProgressReporter : IAnalyzeProgressReporter
{
    private readonly object _gitOperationLock = new();
    private Stopwatch? _gitOperationTimer;

    private readonly object _detectionLock = new();
    private Stopwatch? _detectionTimer;

    private readonly IDictionary<HistoryStopPointOperation, object> _operationLocks =
        new Dictionary<HistoryStopPointOperation, object>
        {
            {HistoryStopPointOperation.Archive, new()},
            {HistoryStopPointOperation.Process, new()}
        };

    private readonly record struct OperationStatus(int Completed, int Total);

    private readonly IDictionary<HistoryStopPointOperation, OperationStatus> _operations =
        new Dictionary<HistoryStopPointOperation, OperationStatus>
        {
            {HistoryStopPointOperation.Archive, new OperationStatus(0, 0)},
            {HistoryStopPointOperation.Process, new OperationStatus(0, 0)}
        };

    [MemberNotNull(nameof(_gitOperationTimer))]
    private void EnsureGitOperationTimer()
    {
        lock (_gitOperationLock)
        {
            _gitOperationTimer = new Stopwatch();
            _gitOperationTimer.Start();
        }
    }

    [MemberNotNull(nameof(_detectionTimer))]
    private void EnsureDetectionTimer()
    {
        lock (_detectionLock)
        {
            _detectionTimer = new Stopwatch();
            _detectionTimer.Start();
        }
    }

    public void ReportGitOperationStarted(GitOperation operation)
    {
        EnsureGitOperationTimer();
        Console.Out.WriteLineAsync($"Git {operation} started.");
    }

    public void ReportGitOperationFinished(GitOperation operation)
    {
        EnsureGitOperationTimer();
        _gitOperationTimer.Stop();
        Console.Out.WriteLineAsync($"Git {operation} finished. Took {_gitOperationTimer.ElapsedMilliseconds}ms.");
    }

    public void ReportHistoryStopPointDetectionStarted()
    {
        EnsureDetectionTimer();
        Console.Out.WriteLineAsync($"History stop point detection started.");
    }

    public void ReportHistoryStopPointDetectionFinished()
    {
        EnsureDetectionTimer();
        _detectionTimer.Stop();
        Console.Out.WriteLineAsync(
            $"History stop point detection finished. Took {_detectionTimer.ElapsedMilliseconds}ms.");
    }

    public void ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation operation, int count)
    {
        lock (_operationLocks[operation])
        {
            _operations[operation] = _operations[operation] with { Total = count };
            Console.Out.WriteLineAsync(
                $"History stop point {operation} started. Expecting {count} operations to complete.");
        }
    }

    public void ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation operation)
    {
        lock (_operationLocks[operation])
        {
            _operations[operation] = _operations[operation] with { Completed = _operations[operation].Completed + 1 };
            Console.Out.WriteLineAsync(
                $"{_operations[operation].Completed}/{_operations[operation].Total} history stop point " +
                $"{operation} completed.");
        }
    }
}
