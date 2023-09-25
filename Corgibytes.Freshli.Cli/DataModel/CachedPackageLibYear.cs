using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(PackageUrl), nameof(AsOfDateTime), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedPackageLibYear : TimeStampedEntity
{
    [Required] public int Id { get; set; }

    [Required] public string PackageUrl { get; set; } = null!;
    [Required] public DateTimeOffset AsOfDateTime { get; set; }

    public string PackageName
    {
        get
        {
            return new PackageURL(PackageUrl).Name;
        }
    }

    public string CurrentVersion
    {
        get
        {
            return new PackageURL(PackageUrl).Version;
        }
    }

    public DateTimeOffset ReleaseDateCurrentVersion { get; set; }
    public string? LatestVersion { get; set; }
    public DateTimeOffset ReleaseDateLatestVersion { get; set; }

    [Required] public double LibYear { get; set; }

    public virtual List<CachedManifest> Manifests { get; } = new();

    public bool DoesBelongTo(CachedManifest manifest)
    {
        return Manifests.Any(m => m.Id == manifest.Id);
    }
}
