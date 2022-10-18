using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedPackageLibYear
{
    [Required] public int Id { get; set; }

    public string? PackageName { get; set; }
    public string? CurrentVersion { get; set; }
    public DateTimeOffset ReleaseDateCurrentVersion { get; set; }
    public string? LatestVersion { get; set; }
    public DateTimeOffset ReleaseDateLatestVersion { get; set; }

    [Required] public double LibYear { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    [Required] public int HistoryStopPointId { get; set; }
    public virtual CachedHistoryStopPoint HistoryStopPoint { get; set; } = null!;
}
