using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedManifest : TimeStampedEntity
{
    [Required] public int Id { get; set; }

    // TODO: Add a validation to ensure that the ManifestFilePath is contained within the HistoryStopPoint.LocalPath
    [Required] public string ManifestFilePath { get; set; } = null!;

    public virtual CachedHistoryStopPoint HistoryStopPoint { get; set; } = null!;

    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public virtual List<CachedPackageLibYear> PackageLibYears { get; init; } = new();
}
