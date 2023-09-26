// ReSharper disable UnusedMemberInSuper.Global

using System;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public interface IHistoryStopData
{
    public string RepositoryId { get; }
    public string? CommitId { get; }
    public DateTimeOffset? CommittedAt { get; }
    public string Path { get; }
    public DateTimeOffset AsOfDateTime { get; }
}
