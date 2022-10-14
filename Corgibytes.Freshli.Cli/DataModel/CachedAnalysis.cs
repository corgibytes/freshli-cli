using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedAnalysis
{
    public CachedAnalysis(string repositoryUrl, string? repositoryBranch, string historyInterval,
        CommitHistory useCommitHistory, RevisionHistoryMode revisionHistoryMode)
    {
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;
        UseCommitHistory = useCommitHistory;
        RevisionHistoryMode = revisionHistoryMode;
    }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    [Required] public Guid Id { get; set; }
    [Required] public string RepositoryUrl { get; set; }

    public string? RepositoryBranch { get; set; }
    public CommitHistory UseCommitHistory { get; set; }
    public RevisionHistoryMode RevisionHistoryMode { get; set; }

    // TODO: Research how to use a value class here instead of a string
    [Required] public string HistoryInterval { get; set; }

    public Guid? ApiAnalysisId { get; set; }

    // ReSharper disable once CollectionNeverQueried.Global
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once UnusedMember.Global
    public virtual List<CachedHistoryStopPoint> HistoryStopPoints { get; set; } = null!;
}
