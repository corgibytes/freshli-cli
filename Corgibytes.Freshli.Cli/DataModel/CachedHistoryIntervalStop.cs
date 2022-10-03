using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
[Table("CachedHistoryIntervalStops")]
public class CachedHistoryIntervalStop
{
    [Required] public int Id { get; set; }

    [Required] public DateTimeOffset GitCommitDate { get; set; }

    [Required] public string GitCommitId { get; set; } = null!;

    [Required] public Guid CachedAnalysisId { get; set; }

    public virtual CachedAnalysis CachedAnalysis { get; set; } = null!;
}
