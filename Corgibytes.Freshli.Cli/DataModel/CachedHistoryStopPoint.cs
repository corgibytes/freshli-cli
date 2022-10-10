using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedHistoryStopPoint
{
    [Required] public int Id { get; set; }

    [Required] public DateTimeOffset AsOfDateTime { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    [Required] public string GitCommitId { get; set; } = null!;

    [Required] public Guid CachedAnalysisId { get; set; }

    // ReSharper disable once UnusedMember.Global
    public virtual CachedAnalysis CachedAnalysis { get; set; } = null!;
}
