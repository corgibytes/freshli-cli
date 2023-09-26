using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedHistoryStopPoint : TimeStampedEntity
{
    [Required] public int Id { get; set; }

    [Required] public DateTimeOffset AsOfDateTime { get; set; }

    [Required] public string RepositoryId { get; set; } = null!;
    [Required] public string LocalPath { get; set; } = null!;

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    [Required] public string GitCommitId { get; set; } = null!;
    [Required] public DateTimeOffset GitCommitDateTime { get; set; }

    [Required] public Guid CachedAnalysisId { get; set; }

    // ReSharper disable once UnusedMember.Global
    public virtual CachedAnalysis CachedAnalysis { get; set; } = null!;

    public virtual CachedGitSource Repository { get; set; } = null!;

    public virtual List<CachedManifest> Manifests { get; } = new();
}
