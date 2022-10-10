using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedLibYear
{
    [Required] public int Id { get; set; }

    public string? PackageName { get; set; }
    public string? CurrentVersion { get; set; }
    public DateTimeOffset ReleaseDateCurrentVersion { get; set; }
    public string? LatestVersion { get; set; }
    public DateTimeOffset ReleaseDateLatestVersion { get; set; }

    [Required] public double LibYear { get; set; }

    [Required] public int HistoryIntervalStopId { get; set; }
    // ReSharper disable once UnusedMember.Global
    public virtual CachedHistoryIntervalStop HistoryIntervalStop { get; set; } = null!;
}
